using AdrianTube2.Models.Movie;

namespace AdrianTube2.state.Interfaces;

public interface ICommentState
{
    public event Action? OnChange;

    public int CommentPage { get; set; }

    public List<Comment> Comments { get; set; }

    public void NextCommentPage();

    public void PreviousCommentePage();

    public void ResetCommentPage();

    public void AddComment(Comment comment);
    public void RemoveComment(Comment comment);

    public void AddComments(List<Comment> comments);

    public void EditComment(Comment comment);

    public void ClearComments();
    
}