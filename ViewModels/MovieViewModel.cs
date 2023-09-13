using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using AdrianTube2.Models.Movie;
using MongoDB.Bson;

namespace AdrianTube2.ViewModels;

public class MovieViewModel
{
    [Required(ErrorMessage = "Please enter a title for the movie")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters long")]
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string CategoryId { get; set; } = "";
    public IBrowserFile Thumbnail { get; set; }
    public IBrowserFile Video { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public string VideoUrl { get; set; } = "";
    public string ThumbnailUrl { get; set; } = "";
    public ObjectId Id { get; set; }
    public bool IsShort { get; set; } = false;

    public MovieViewModel(Movie movie)
    {
        Title = movie.Title;
        Description = movie.Description;
        Tags = movie.Tags;
        VideoUrl = movie.VideoUrl;
        Id = movie.Id;
        ThumbnailUrl = movie.Thumbnail;
    }

    public MovieViewModel()
    {
    }
}
