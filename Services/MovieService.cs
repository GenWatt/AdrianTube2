using System.Security.Claims;
using AdrianTube2.Models.Movie;
using AdrianTube2.Models.UserModels;
using AdrianTube2.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NReco.VideoInfo;

namespace AdrianTube2.Services;

public class MovieService
{
    private AuthenticationStateProvider _authenticationStateProvider { get; set; }
    private readonly MongoClient _client = new (Environment.GetEnvironmentVariable("MONGO_URI")!);
    private IMongoCollection<Movie> _moviesCollection { get; set; }
    private IMongoCollection<User> _usersCollection { get; set; }
    private IMongoCollection<View> _viewsCollection { get; set; }
    private IMongoCollection<Subscribtion> _subscriptionsCollection { get; set; }
    private FileService _fileService { get; set; }
    private CommentService _commentService { get; set; }
    private LikeService _likeService { get; set; }
    public string ThumbnailDirectory { get; set; } = Constants.ThumbnailDirectory;
    public string VideoDirectory { get; set; } = Constants.VideoDirectory;

    public MovieService(AuthenticationStateProvider authenticationStateProvider, FileService fileService, CommentService commentService, LikeService likeService)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _fileService = fileService;
        _likeService = likeService;
        _commentService = commentService;
        _moviesCollection = _client.GetDatabase("AdrianTube").GetCollection<Movie>("Movies");
        _usersCollection = _client.GetDatabase("AdrianAuth").GetCollection<User>("users");
        _viewsCollection = _client.GetDatabase("AdrianTube").GetCollection<View>("Views");
        _subscriptionsCollection = _client.GetDatabase("AdrianTube").GetCollection<Subscribtion>("Subscriptions");
    }

    public async Task<string> SaveThumbnail(IBrowserFile file)
    {
        var newName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
        _fileService.CreateDirectory(ThumbnailDirectory);
        var filePath = $"{ThumbnailDirectory}/{newName}";

        await using FileStream fs = new(filePath, FileMode.Create);
        await file.OpenReadStream().CopyToAsync(fs);

        return newName;
    }

    public async Task<string> SaveMovie(IBrowserFile file)
    {
        var newName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
        _fileService.CreateDirectory(VideoDirectory);
        var filePath = $"{VideoDirectory}/{newName}";

        await using FileStream fs = new(filePath, FileMode.Create);
        await file.OpenReadStream((long)Constants.MaxVideoSize).CopyToAsync(fs);

        return newName;
    }

    public void DeleteThumbnail(string name)
    {
        var filePath = $"{ThumbnailDirectory}/{name}";
        _fileService.DeleteFileIfExists(name);
    }

    public void DeleteMovie(string name)
    {
        var filePath = $"{VideoDirectory}/{name}";
        _fileService.DeleteFileIfExists(name);
    }

    public MediaInfo GetVideoInfo(string path) {
        var ffmpeg = new FFProbe();
        var videoInfo = ffmpeg.GetMediaInfo(path);
        
        return videoInfo;
    }

    public async Task<Movie> AddMovie(MovieViewModel movieViewModel)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            var thumbnailName = await SaveThumbnail(movieViewModel.Thumbnail);
            var videoName = await SaveMovie(movieViewModel.Video);
            var videoInfo = GetVideoInfo($"{VideoDirectory}/{videoName}");
            var userFromDb = await _usersCollection.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();

            var movie = new Movie
            {
                Title = movieViewModel.Title,
                Description = movieViewModel.Description,
                Thumbnail = $"static/thumbnails/{thumbnailName}",
                VideoUrl = $"static/videos/{videoName}",
                Duration = videoInfo.Duration,
                User = userFromDb,
                UserId = new ObjectId(userId)
            };
            await _moviesCollection.InsertOneAsync(movie);

            return movie;
        }

        throw new Exception("User not logged in!");
    }

    public async Task<Movie> UpdateMovie(MovieViewModel movieViewModel) {
        var movie = await _moviesCollection.Find(m => m.Id == movieViewModel.Id).FirstOrDefaultAsync();

        if (movie != null) {
            movie.Title = movieViewModel.Title;
            movie.Description = movieViewModel.Description;
            movie.Tags = movieViewModel.Tags;
            // check if thumbnail is updated
            if (movieViewModel.Thumbnail != null && movieViewModel.Thumbnail.Name != movie.Thumbnail) {
                var thumbnailName = await SaveThumbnail(movieViewModel.Thumbnail);
                DeleteThumbnail($"{ThumbnailDirectory}/{movie.Thumbnail.Split("/").Last()}");
                movie.Thumbnail = $"static/thumbnails/{thumbnailName}";
            }

            // check if video is updated
            if (movieViewModel.Video != null && movieViewModel.Video.Name != movie.VideoUrl) {
                var videoName = await SaveMovie(movieViewModel.Video);
                DeleteMovie($"{VideoDirectory}/{movie.VideoUrl.Split("/").Last()}");
                movie.VideoUrl = $"static/videos/{videoName}";
            }

            await _moviesCollection.ReplaceOneAsync(m => m.Id == movie.Id, movie);

            return movie;
        }

        throw new Exception("Movie not found!");
    }

    public async Task<List<Movie>> GetMovies(int page = 1, int pageSize = 20)
    {
        var movies = await _moviesCollection.Find(m => true).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();

        return movies;
    }

    public async Task<Movie?> GetMovieById(string id) {
        var movie = await _moviesCollection.Find(m => m.Id == new ObjectId(id)).FirstOrDefaultAsync();
        var movieCreator = await _usersCollection.Find(u => u.Id == movie.UserId).FirstOrDefaultAsync();

        if (movie == null || movieCreator == null) {
            return null;
        }

        movie.User  = movieCreator;
        
        return movie;
    } 

    public async Task<List<Movie>> GetMoviesByTitle(string title) {
        var movies = await _moviesCollection.Find(m => m.Title.ToLower().Contains(title.ToLower())).ToListAsync();

        return movies;
    }

    public async Task DeleteMovie(ObjectId id) {
        var movie = await _moviesCollection.Find(m => m.Id == id).FirstOrDefaultAsync();

        DeleteThumbnail($"{ThumbnailDirectory}/{movie.Thumbnail.Split("/").Last()}");
        DeleteMovie($"{VideoDirectory}/{movie.VideoUrl.Split("/").Last()}");

        await _commentService.DeleteMovieComments(id);
        await _viewsCollection.DeleteManyAsync(v => v.Movie.Id == id);
        await _likeService.DeleteMovieLikesAndDisLikes(id);
        await _moviesCollection.DeleteOneAsync(m => m.Id == id);
    }

    public async Task<List<Movie>> GetMoviesByUserId(string id) {
        var movies = await _moviesCollection.Find(m => m.UserId == new ObjectId(id)).ToListAsync();

        return movies;
    }   

    public async Task AddView(Movie movie) {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null) {
            var dbUser = await _usersCollection.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();
            var isViewed = await _viewsCollection.Find(v => v.Movie.Id == movie.Id && v.User.Id == dbUser.Id).AnyAsync();

            if (isViewed) return;
            
            var view = new View {
                Movie = movie,
                User = dbUser
            };

            await _viewsCollection.InsertOneAsync(view);
        }
    }

    public async Task<int> GetViewsCount(ObjectId id) {
        var viewsCount = await _viewsCollection.Find(v => v.Movie.Id == id).CountDocumentsAsync();

        return (int)viewsCount;
    }
}