using AdrianTube2.Models.Movie;
using AdrianTube2.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson;
using NReco.VideoInfo;

namespace AdrianTube2.Services.Interfaces;

public interface IMovieService
{
    public Task<string> SaveThumbnail(IBrowserFile file);

    public Task<string> SaveMovie(IBrowserFile file);

    public void DeleteThumbnail(string name);

    public void DeleteMovie(string name);
    public MediaInfo GetVideoInfo(string path);

    public decimal GetVideoAspectRatio(string path);

    public Task<Movie> AddMovie(MovieViewModel movieViewModel);

    public Task<Movie> UpdateMovie(MovieViewModel movieViewModel);

    public Task<List<Movie>> GetMovies(int page = 1, int pageSize = 20);

    public Task<List<Movie>> GetShorts(int page = 1, int pageSize = 20);

    public Task<Movie?> GetMovieById(string id);

    public Task<List<Movie>> GetMoviesByTitle(string title);

    public Task DeleteMovie(ObjectId id);

    public Task<List<Movie>> GetMoviesByUserId(string id, int page = 1, int pageSize = 20);
    public Task<List<Movie>> GetShortsByUserId(string id, int page = 1, int pageSize = 20);

    public Task AddView(Movie movie);

    public Task<int> GetViewsCount(ObjectId id);
}