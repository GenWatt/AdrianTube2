using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using AdrianTube2.Models.UserModels;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace AdrianTube2.Models.Movie;

public class Like
{
    public ObjectId Id { get; set; }
    public Movie Movie { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Dislike
{
    public ObjectId Id { get; set; }
    public Movie Movie { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Comment
{
    public ObjectId Id { get; set; }
    public Movie Movie { get; set; }
    public User User { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class View
{
    public ObjectId Id { get; set; }
    public Movie Movie { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Subscribtion
{
    public ObjectId Id { get; set; }
    public User User { get; set; }
    public User Creator { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Movie
{
    [JsonPropertyName("_id")]
    public ObjectId Id { get; set; }
    [Required(ErrorMessage = "Please enter a title for the movie")]
    [StringLength(3, ErrorMessage = "Title must be at least 3 characters long")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Please enter a description for the movie")]
    public string Description { get; set; }
    [JsonPropertyName("_id")]
    public ObjectId CategoryId { get; set; }
    public string Thumbnail { get; set; }
    public string VideoUrl { get; set; }
    [JsonPropertyName("_id")]
    public ObjectId UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<string> Tags { get; set; } = new List<string>();
    public List<Like> Likes { get; set; } = new List<Like>();
    public List<Dislike> Dislikes { get; set; } = new List<Dislike>();
    public TimeSpan Duration { get; set; }
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<View> Views { get; set; } = new List<View>();
    [BsonIgnoreIfNull]
    public bool IsShort { get; set; }
}

