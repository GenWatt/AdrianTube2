using System.Security.Claims;
using AdrianTube2.Models.Movie;
using AdrianTube2.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using MongoDB.Driver;

namespace AdrianTube2.Services;

public class MovieService
{
    private AuthenticationStateProvider _authenticationStateProvider { get; set; }
    private readonly MongoClient _client = new MongoClient(Environment.GetEnvironmentVariable("MONGO_URI")!);
    private IMongoCollection<Movie> _movies { get; set; }

    public MovieService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _movies = _client.GetDatabase("AdrianTube").GetCollection<Movie>("Movies");
    }

    public async Task<Movie> AddMovie(MovieViewModel movieViewModel)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var movie = new Movie
        {
            Title = movieViewModel.Title,
            Description = movieViewModel.Description,
            // Thumbnail = movieViewModel.Thumbnail
            // VideoUrl = movieViewModel.Video,
        };

        // var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", movieViewModel.Thumbnail.Name);

        // await using FileStream fs = new(path, FileMode.Create);
        // await movieViewModel.Thumbnail.OpenReadStream().CopyToAsync(fs);

        var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", movieViewModel.Video.Name);

        await using FileStream videoFs = new(videoPath, FileMode.Create);
        await movieViewModel.Video.OpenReadStream(1024 * 1024 * 100).CopyToAsync(videoFs);

        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            throw new Exception("User not logged in!");
        }

        movie.UserId = new MongoDB.Bson.ObjectId(userId);
        await _movies.InsertOneAsync(movie);

        return movie;
    }
}