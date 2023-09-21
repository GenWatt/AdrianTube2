using AdrianTube2.Models.Movie;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AdrianTube2.Services.Interfaces;

public interface ILikeService
{
    public Task<LikeStatus> InsertLike(Movie movie, string userId);

    public Task<LikeStatus> InsertDislike(Movie movie, string userId);

    public Task<LikeStatus> AddLike(Movie movie);

    public Task<LikeStatus> AddDislike(Movie movie);

    public Task DeleteLike(ObjectId movieId, string userId, IClientSessionHandle? session = null);

    public Task DeleteDislike(Movie movie, string userId, IClientSessionHandle? session = null);

    public Task<int> GetMovieLikes(ObjectId movieId);

    public Task<int> GetMovieDislikes(ObjectId movieId);

    public Task DeleteMovieLikesAndDisLikes(ObjectId movieId);
}