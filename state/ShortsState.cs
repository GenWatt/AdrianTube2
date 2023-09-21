using AdrianTube2.Models.Movie;
using AdrianTube2.Services.Interfaces;
using AdrianTube2.state.Interfaces;

namespace AdrianTube2.state;

public class ShortsState : IShortsState
{
    private List<Movie> _shorts = new ();
    private int _shortPage = 1;
    public event Action? OnChange;
    private Movie? _currentShort { get; set; } = null;
    private int _currentShortIndex { get; set; } = 0;
    private readonly IMovieService _movieService;
    private int _loadMoreCountMargin = 4;
    
    public ShortsState(IMovieService movieService)
    {
        _movieService = movieService;
    }
    public Movie? CurrentShort
    {
        get { return _currentShort; }
        set
        {
            _currentShort = value;
            NotifyStateChanged();
        }
    }

    public int ShortPage
    {
        get { return _shortPage; }
        set
        {
            _shortPage = value;
            NotifyStateChanged();
        }
    }

    public async Task<Movie?> NextShort() {
        if (_currentShortIndex >= _shorts.Count - _loadMoreCountMargin) {
            _shortPage++;
            await _movieService.GetShorts(_shortPage);
        }

        if (_currentShortIndex < _shorts.Count - 1)
        {
            _currentShortIndex++;
            CurrentShort = _shorts[_currentShortIndex];
            NotifyStateChanged();
            return _currentShort;
        }

        return null;
    }

    public Movie? PreviousShort() {
        if (_currentShortIndex > 0)
        {
            _currentShortIndex--;
            _currentShort = _shorts[_currentShortIndex];
            NotifyStateChanged();

            return _currentShort;
        }

        return null;
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

    public void ResetShortes() {
        _shorts = new List<Movie>();
        ResetShortePage();
        _currentShortIndex = 0;
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