
using AdrianTube2.Models.Movie;

namespace AdrianTube2.state;

public class StateContainer
{
    private List<Movie> _movies = new List<Movie>();
    public event Action? OnChange;

    public List<Movie> Movies
    {
        get { return _movies; }
        set
        {
            _movies = value;
            NotifyStateChanged();
        }
    }

    public void AddMovie(Movie movie)
    {
        _movies.Add(movie);
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}