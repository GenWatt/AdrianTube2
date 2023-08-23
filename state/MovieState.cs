using AdrianTube2.Models.Movie;

namespace AdrianTube2.state;

public class MovieState
{
    private List<Movie> _movies = new List<Movie>();
    private int _moviePage = 1;
    public event Action? OnChange;

    public int MoviePage
    {
        get { return _moviePage; }
        set
        {
            _moviePage = value;
            NotifyStateChanged();
        }
    }

    public void NextMoviePage()
    {
        _moviePage++;
        NotifyStateChanged();
    }

    public void PreviousMoviePage()
    {
        _moviePage--;
        NotifyStateChanged();
    }

    public void ResetMoviePage()
    {
        _moviePage = 1;
        NotifyStateChanged();
    }

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

    public void RemoveMovie(Movie movie)
    {
        _movies.Remove(movie);
        NotifyStateChanged();
    }

    public void AddMovies(List<Movie> movies)
    {
        _movies.AddRange(movies);
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}