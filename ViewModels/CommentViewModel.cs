using System.ComponentModel.DataAnnotations;
using AdrianTube2.Models.Movie;
using MongoDB.Bson;

namespace AdrianTube2.ViewModels;

public class CommentViewModel
{
    [Required]
    [MinLength(3)]
    [MaxLength(500, ErrorMessage = "Comment cannot be longer than 500 characters")]
    public string Comment { get; set; } = string.Empty;
    public Movie Movie { get; set; }
    public ObjectId Id { get; set; }
}
