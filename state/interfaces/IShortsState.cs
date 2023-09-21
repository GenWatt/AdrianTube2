
using AdrianTube2.Models.Movie;

namespace AdrianTube2.state.Interfaces;

public interface IShortsState
{
    public Movie? CurrentShort { get; set; }
    public int ShortPage { get; set; }

    public Task<Movie?> NextShort();

    public Movie? PreviousShort();

    public void NextShortPage();

    public void PreviousShortPage();

    public void ResetShortePage();
    public void ResetShortes();

    public List<Movie> Shorts { get; set; }

    public void AddShort(Movie shortMovie);

    public void RemoveShort(Movie shortMovie);
    public void AddShorts(List<Movie> shortMovie);

    public void UpdateShort(Movie shortMovie);
    public event Action? OnChange;
}