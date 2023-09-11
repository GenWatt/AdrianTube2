using System.Security.Claims;
using AdrianTube2.Models.Movie;
using AdrianTube2.Models.UserModels;
using Microsoft.AspNetCore.Components.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AdrianTube2.Services;

public enum LikeStatus {
    Liked,
    LikedAndUnDisliked,
    UnLiked,
    Disliked,
    DislikedAndUnLiked,
    UnDisliked
}

public class LikeService
{
    private AuthenticationStateProvider _authenticationStateProvider { get; set; }
    private readonly MongoClient _client = new (Environment.GetEnvironmentVariable("MONGO_URI")!);
    private IMongoCollection<Like> _likesCollection { get; set; }
    private IMongoCollection<Dislike> _dislikesCollection { get; set; }
    private IMongoCollection<User> _userCollection { get; set; }

    public LikeService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _likesCollection = _client.GetDatabase("AdrianTube").GetCollection<Like>("Likes");
        _dislikesCollection = _client.GetDatabase("AdrianTube").GetCollection<Dislike>("DisLikes");
        _userCollection = _client.GetDatabase("AdrianAuth").GetCollection<User>("users");
    }

    public async Task<LikeStatus> InsertLike(Movie movie, string userId) {
        var status = LikeStatus.Liked;
        using var session = await _client.StartSessionAsync();
        session.StartTransaction();

        var dbUser = await _userCollection.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();

        if (dbUser == null) {
            throw new Exception("User not found");
        }

        var dislike = await _dislikesCollection.Find(l => l.Movie.Id == movie.Id && l.User.Id == new ObjectId(userId)).FirstOrDefaultAsync();

        if (dislike != null) {
            await DeleteDislike(movie, userId, session);
            status = LikeStatus.LikedAndUnDisliked;  
        }
        await _likesCollection.InsertOneAsync(session, new Like
        {
            Movie = movie,
            User = dbUser
        });

        await session.CommitTransactionAsync();

        return status;
    }

    public async Task<LikeStatus> InsertDislike(Movie movie, string userId) {
        var status = LikeStatus.Disliked;

        using var session = await _client.StartSessionAsync();
        session.StartTransaction();

        var like = await _likesCollection.Find(l => l.Movie.Id == movie.Id && l.User.Id == new ObjectId(userId)).FirstOrDefaultAsync();
        var dbUser = await _userCollection.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();

        if (dbUser == null) {
            throw new Exception("User not found");
        }   

        if (like != null) {
            await DeleteLike(movie.Id, userId, session);
            status = LikeStatus.DislikedAndUnLiked;
        }

        await _dislikesCollection.InsertOneAsync(session, new Dislike
        {
            Movie = movie,
            User = dbUser
        });

        await session.CommitTransactionAsync();

        return status;
    }

    public async Task<LikeStatus> AddLike(Movie movie) {
        var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (user != null && userId != null && user.Identity?.IsAuthenticated == true) {
            var like = await _likesCollection.Find(l => l.Movie.Id == movie.Id && l.User.Id == new ObjectId(userId)).FirstOrDefaultAsync();

            if (like == null) {
                return await InsertLike(movie, userId);
            } else {
                await DeleteLike(movie.Id, userId);
                return LikeStatus.UnLiked;
            }
        } 
        throw new Exception("User is not authenticated");
    }

    public async Task<LikeStatus> AddDislike(Movie movie) {
        var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (user != null && userId != null && user.Identity?.IsAuthenticated == true) {
            var dislike = await _dislikesCollection.Find(l => l.Movie.Id == movie.Id && l.User.Id == new ObjectId(userId)).FirstOrDefaultAsync();

            if (dislike == null) {
                return await InsertDislike(movie, userId);
            } else {
                await DeleteDislike(movie, userId);
                return LikeStatus.UnDisliked;
            }
        } 
        throw new Exception("User is not authenticated");
    }

    public async Task DeleteLike(ObjectId movieId, string userId, IClientSessionHandle? session = null) {
        if (session == null) {
            await _likesCollection.DeleteOneAsync(l => l.Movie.Id == movieId && l.User.Id == new ObjectId(userId));
            return;
        }
        await _likesCollection.DeleteOneAsync(l => l.Movie.Id == movieId && l.User.Id == new ObjectId(userId));
    }

    public async Task DeleteDislike(Movie movie, string userId, IClientSessionHandle? session = null) {
        if (session == null) {
            await _dislikesCollection.DeleteOneAsync(l => l.Movie.Id == movie.Id && l.User.Id == new ObjectId(userId));
            return;
        }
        await _dislikesCollection.DeleteOneAsync(session, l => l.Movie.Id == movie.Id && l.User.Id == new ObjectId(userId));
    }

    public async Task<int> GetMovieLikes(ObjectId movieId) {
        var result = (int) await _likesCollection.CountDocumentsAsync(l => l.Movie.Id == movieId);
        return result;
    }

    public async Task<int> GetMovieDislikes(ObjectId movieId) {
        var result = (int) await _dislikesCollection.CountDocumentsAsync(l => l.Movie.Id == movieId);
        return result;
    }

    public async Task DeleteMovieLikesAndDisLikes(ObjectId movieId) {
        await _likesCollection.DeleteManyAsync(l => l.Movie.Id == movieId);
        await _dislikesCollection.DeleteManyAsync(l => l.Movie.Id == movieId);
    }
}