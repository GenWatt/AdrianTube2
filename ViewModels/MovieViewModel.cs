using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

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
}
