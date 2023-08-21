using System.ComponentModel.DataAnnotations;
using AdrianTube2.Models.Movie;

namespace AdrianTube2.ViewModels;

public class CommentViewModel
{
    [Required]
    [MinLength(3)]
    [MaxLength(500)]
    public string Comment { get; set; } = string.Empty;
    public Movie Movie { get; set; }
}
