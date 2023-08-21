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

public enum SubscriptionResult {
    Subscribed,
    UnSubscribed
}

public class MovieService
{
    private AuthenticationStateProvider _authenticationStateProvider { get; set; }
    private readonly MongoClient _client = new (Environment.GetEnvironmentVariable("MONGO_URI")!);
    private IMongoCollection<Movie> _moviesCollection { get; set; }
    private IMongoCollection<User> _usersCollection { get; set; }
    private IMongoCollection<View> _viewsCollection { get; set; }
    private IMongoCollection<Subscribtion> _subscriptionsCollection { get; set; }
    private FileService _fileService { get; set; } = new();
    public string ThumbnailDirectory { get; set; } = Constants.ThumbnailDirectory;
    public string VideoDirectory { get; set; } = Constants.VideoDirectory;

    public MovieService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
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

    public async Task<List<Movie>> GetMovies(int page = 1, int pageSize = 6)
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
        await _moviesCollection.DeleteOneAsync(m => m.Id == id);
    }

    public async Task<List<Movie>> GetMoviesByUserId(string id) {
        var movies = await _moviesCollection.Find(m => m.UserId == new ObjectId(id)).ToListAsync();

        return movies;
    }   

    public async Task AddView(string id) {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null) {
            var isViewed = await _viewsCollection.Find(v => v.MovieId == new ObjectId(id) && v.UserId == new ObjectId(userId)).AnyAsync();

            if (isViewed) return;
            
            var view = new View {
                MovieId = new ObjectId(id),
                UserId = new ObjectId(userId)
            };

            await _viewsCollection.InsertOneAsync(view);
        }
    }

    public async Task<int> GetViewsCount(ObjectId id) {
        var viewsCount = await _viewsCollection.Find(v => v.MovieId == id).CountDocumentsAsync();

        return (int)viewsCount;
    }

    public async Task<SubscriptionResult> SubscribeOrUnSubscribe(string creatorId) {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null) {
            var isSubscribed = await _subscriptionsCollection.Find(s => s.CreatorId == new ObjectId(creatorId) && s.UserId == new ObjectId(userId)).AnyAsync();

            if (isSubscribed) {
                await _subscriptionsCollection.DeleteOneAsync(s => s.CreatorId == new ObjectId(creatorId) && s.UserId == new ObjectId(userId));
                return SubscriptionResult.UnSubscribed;
            } else {
                var subscription = new Subscribtion {
                    CreatorId = new ObjectId(creatorId),
                    UserId = new ObjectId(userId)
                };

                await _subscriptionsCollection.InsertOneAsync(subscription);
                return SubscriptionResult.Subscribed;
            }
        }
        throw new Exception("User not logged in!");
    }

    public async Task<int> GetSubscriptionCount(ObjectId id) {
        var subscriptionCount = await _subscriptionsCollection.Find(s => s.CreatorId == id).CountDocumentsAsync();

        return (int)subscriptionCount;
    }

    public async Task<bool> IsSubscribing(ObjectId creatorId) {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null) {
            var isSubscribed = await _subscriptionsCollection.Find(s => s.CreatorId == creatorId && s.UserId == new ObjectId(userId)).AnyAsync();

            return isSubscribed;
        }

        return false;
    }
}