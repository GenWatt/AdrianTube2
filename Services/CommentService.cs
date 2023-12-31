using System.Security.Claims;
using AdrianTube2.Models.Movie;
using AdrianTube2.Services.Interfaces;
using AdrianTube2.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AdrianTube2.Services;

public class CommentService : ICommentService {
    private AuthenticationStateProvider _authenticationStateProvider { get; set; }
    private readonly MongoClient _client = new (Environment.GetEnvironmentVariable("MONGO_URI")!);
    private IMongoCollection<Comment> _commentCollection { get; set; }
    private IUserService _userService { get; set; }

    public CommentService(AuthenticationStateProvider authenticationStateProvider, IUserService userService) {
        _userService = userService;
        _authenticationStateProvider = authenticationStateProvider;
        _commentCollection = _client.GetDatabase("AdrianTube").GetCollection<Comment>("Comments");
    }

    public async Task<Comment> CreateComment(CommentViewModel commentViewModel) {
        if (commentViewModel == null) {
            throw new Exception("Comment cannot be null");
        }

        if (commentViewModel.Comment.Length < 3) {
            throw new Exception("Comment cannot be shorter than 3 characters");
        }

        if (commentViewModel.Comment.Length > 500) {
            throw new Exception("Comment cannot be longer than 500 characters");
        }

        var currentUser = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
        var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userService.GetUser(ObjectId.Parse(userId));
        
        if (user == null) {
            throw new Exception("Wrong user id");
        }

        var comment = new Comment {
            Text = commentViewModel.Comment,
            Movie = commentViewModel.Movie,
            User = user,
        };

        await _commentCollection.InsertOneAsync(comment);

        return comment;
    }

    public async Task<List<Comment>> GetComments(ObjectId movieId, int page = 1, int pageSize = 5) {
        var comments = await _commentCollection.Find(comment => comment.Movie.Id == movieId).Skip((page - 1) * pageSize).Limit(pageSize).ToListAsync();

        return comments;
    }

    public async Task DeleteComment(ObjectId commentId) {
        await _commentCollection.DeleteOneAsync(comment => comment.Id == commentId);
    }

    public async Task<Comment> EditComment(CommentViewModel commentViewModel) {
        var comment = await _commentCollection.Find(comment => comment.Id == commentViewModel.Id).FirstOrDefaultAsync();

        if (comment == null) {
            throw new Exception("Comment not found");
        }

        comment.Text = commentViewModel.Comment;

        await _commentCollection.ReplaceOneAsync(comment => comment.Id == commentViewModel.Id, comment);

        return comment;
    }

    public async Task DeleteMovieComments(ObjectId movieId) {
        await _commentCollection.DeleteManyAsync(comment => comment.Movie.Id == movieId);
    }
}