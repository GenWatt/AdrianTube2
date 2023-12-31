using AdrianTube2.Models.Movie;
using AdrianTube2.state.Interfaces;

namespace AdrianTube2.state;

public class MovieState : IMovieState
{
    private List<Movie> _movies = new ();
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

    public void  UpdateMovie(Movie movie)
    {
        var index = _movies.FindIndex(m => m.Id == movie.Id);
        _movies[index] = movie;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}