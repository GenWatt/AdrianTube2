using AdrianTube2.Models.Movie;
using AdrianTube2.ViewModels;
using MongoDB.Bson;

namespace AdrianTube2.Services.Interfaces;

public interface ICommentService
{
    public  Task<Comment> CreateComment(CommentViewModel commentViewModel);
    public Task<List<Comment>> GetComments(ObjectId movieId, int page = 1, int pageSize = 5);

    public Task DeleteComment(ObjectId commentId);

    public Task<Comment> EditComment(CommentViewModel commentViewModel);
    public Task DeleteMovieComments(ObjectId movieId);
}