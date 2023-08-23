using AdrianTube2.Models.Movie;

namespace AdrianTube2.state;

public class CommentState
{

    private List<Comment> _comments = new List<Comment>();
    private int _commentPage = 1;
    public event Action? OnChange;

    public int CommentPage
    {
        get { return _commentPage; }
        set
        {
            _commentPage = value;
            NotifyStateChanged();
        }
    }

    public List<Comment> Comments
    {
        get { return _comments; }
        set
        {
            _comments = value;
            NotifyStateChanged();
        }
    }

    public void NextCommentPage()
    {
        _commentPage++;
        NotifyStateChanged();
    }

    public void PreviousCommentePage()
    {
        _commentPage--;
        NotifyStateChanged();
    }

    public void ResetCommentPage()
    {
        _commentPage = 1;
        NotifyStateChanged();
    }

    public void AddComment(Comment comment)
    {
        _comments.Add(comment);
        NotifyStateChanged();
    }

    public void RemoveComment(Comment comment)
    {
        _comments.Remove(comment);
        NotifyStateChanged();
    }

    public void AddComments(List<Comment> comments)
    {
        _comments.AddRange(comments);
        NotifyStateChanged();
    }

    public void EditComment(Comment comment)
    {
        var index = _comments.FindIndex(c => c.Id == comment.Id);
        _comments[index] = comment;
        NotifyStateChanged();
    }

    public void ClearComments()
    {
        _comments.Clear();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}