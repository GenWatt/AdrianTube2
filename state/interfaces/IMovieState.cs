using AdrianTube2.Models.Movie;

namespace AdrianTube2.state.Interfaces;

public interface IMovieState
{
    public event Action? OnChange;

    public int MoviePage { get; set; }

    public void NextMoviePage();

    public void PreviousMoviePage();

    public void ResetMoviePage();

    public List<Movie> Movies { get; set; }

    public void AddMovie(Movie movie);

    public void RemoveMovie(Movie movie);

    public void AddMovies(List<Movie> movies);

    public void UpdateMovie(Movie movie);
}