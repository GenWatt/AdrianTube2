using AdrianTube2.Models.Movie;

namespace AdrianTube2.state;

public class ShortsState
{
    private List<Movie> _shorts = new ();
    private int _shortPage = 1;
    public event Action? OnChange;

    public int ShortPage
    {
        get { return _shortPage; }
        set
        {
            _shortPage = value;
            NotifyStateChanged();
        }
    }

    public void NextShortPage()
    {
        _shortPage++;
        NotifyStateChanged();
    }

    public void PreviousShortPage()
    {
        _shortPage--;
        NotifyStateChanged();
    }

    public void ResetShortePage()
    {
        _shortPage = 1;
        NotifyStateChanged();
    }

    public List<Movie> Shorts
    {
        get { return _shorts; }
        set
        {
            _shorts = value;
            NotifyStateChanged();
        }
    }

    public void AddShort(Movie shortMovie)
    {
        _shorts.Add(shortMovie);
        NotifyStateChanged();
    }

    public void RemoveShort(Movie shortMovie)
    {
        _shorts.Remove(shortMovie);
        NotifyStateChanged();
    }

    public void AddShorts(List<Movie> shortMovie)
    {
        _shorts.AddRange(shortMovie);
        NotifyStateChanged();
    }

    public void UpdateShort(Movie shortMovie)
    {
        var index = _shorts.FindIndex(m => m.Id == shortMovie.Id);
        _shorts[index] = shortMovie;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}