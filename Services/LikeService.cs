using System.Security.Claims;
using AdrianTube2.Models.Movie;
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

    public LikeService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _likesCollection = _client.GetDatabase("AdrianTube").GetCollection<Like>("Likes");
        _dislikesCollection = _client.GetDatabase("AdrianTube").GetCollection<Dislike>("DisLikes");
    }

    public async Task<LikeStatus> InsertLike(ObjectId movieId, string userId) {
        var status = LikeStatus.Liked;
        using var session = await _client.StartSessionAsync();
        session.StartTransaction();

        var dislike = await _dislikesCollection.Find(l => l.MovieId == movieId && l.UserId == new ObjectId(userId)).FirstOrDefaultAsync();

        if (dislike != null) {
            await DeleteDislike(movieId, userId, session);
            status = LikeStatus.LikedAndUnDisliked;  
        }
        await _likesCollection.InsertOneAsync(session, new Like
        {
            MovieId = movieId,
            UserId = new ObjectId(userId)
        });

        await session.CommitTransactionAsync();

        return status;
    }

    public async Task<LikeStatus> InsertDislike(ObjectId movieId, string userId) {
        var status = LikeStatus.Disliked;

        using var session = await _client.StartSessionAsync();
        session.StartTransaction();

        var like = await _likesCollection.Find(l => l.MovieId == movieId && l.UserId == new ObjectId(userId)).FirstOrDefaultAsync();

        if (like != null) {
            await DeleteLike(movieId, userId, session);
            status = LikeStatus.DislikedAndUnLiked;
        }

        await _dislikesCollection.InsertOneAsync(session, new Dislike
        {
            MovieId = movieId,
            UserId = new ObjectId(userId)
        });

        await session.CommitTransactionAsync();

        return status;
    }

    public async Task<LikeStatus> AddLike(ObjectId movieId) {
        var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (user != null && userId != null && user.Identity?.IsAuthenticated == true) {
            var like = await _likesCollection.Find(l => l.MovieId == movieId && l.UserId == new ObjectId(userId)).FirstOrDefaultAsync();

            if (like == null) {
                return await InsertLike(movieId, userId);
            } else {
                await DeleteLike(movieId, userId);
                return LikeStatus.UnLiked;
            }
        } 
        throw new Exception("User is not authenticated");
    }

    public async Task<LikeStatus> AddDislike(ObjectId movieId) {
        var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (user != null && userId != null && user.Identity?.IsAuthenticated == true) {
            var dislike = await _dislikesCollection.Find(l => l.MovieId == movieId && l.UserId == new ObjectId(userId)).FirstOrDefaultAsync();

            if (dislike == null) {
                return await InsertDislike(movieId, userId);
            } else {
                await DeleteDislike(movieId, userId);
                return LikeStatus.UnDisliked;
            }
        } 
        throw new Exception("User is not authenticated");
    }

    public async Task DeleteLike(ObjectId movieId, string userId, IClientSessionHandle? session = null) {
        if (session == null) {
            await _likesCollection.DeleteOneAsync(l => l.MovieId == movieId && l.UserId == new ObjectId(userId));
            return;
        }
        await _likesCollection.DeleteOneAsync(l => l.MovieId == movieId && l.UserId == new ObjectId(userId));
    }

    public async Task DeleteDislike(ObjectId movieId, string userId, IClientSessionHandle? session = null) {
        if (session == null) {
            await _dislikesCollection.DeleteOneAsync(l => l.MovieId == movieId && l.UserId == new ObjectId(userId));
            return;
        }
        await _dislikesCollection.DeleteOneAsync(session, l => l.MovieId == movieId && l.UserId == new ObjectId(userId));
    }

    public async Task<int> GetMovieLikes(ObjectId movieId) {
        var result = (int) await _likesCollection.CountDocumentsAsync(l => l.MovieId == movieId);
        return result;
    }

    public async Task<int> GetMovieDislikes(ObjectId movieId) {
        var result = (int) await _dislikesCollection.CountDocumentsAsync(l => l.MovieId == movieId);
        return result;
    }
}