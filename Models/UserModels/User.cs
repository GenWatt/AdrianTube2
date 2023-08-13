using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AdrianTube2.Models.UserModels;

public class User
{
    [BsonElement("username")]
    public string Username { get; set; }
    [BsonElement("email")]
    public string Email { get; set; }
    [BsonElement("_id")]
    [JsonPropertyName("_id")]
    public ObjectId Id { get; set; }
    [BsonElement("profilePicture")]
    public string ProfilePicture { get; set; }
    [BsonElement("coverPicture")]
    public string CoverPicture { get; set; }
    [BsonElement("isVerified")]
    public bool IsVerified { get; set; }
    [BsonElement("password")]
    public string Password { get; set; }
    [BsonElement("provider")]
    public string Provider { get; set; }
    [BsonElement("__v")]
    public int V { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    [BsonElement("googleId")]
    public string GoogleId { get; set; }
    [BsonElement("active")]
    public bool Active { get; set; }
    [BsonElement("isLogged")]
    public bool IsLogged { get; set; }
    [BsonElement("refreshToken")]
    public string RefreshToken { get; set; }
    [BsonElement("role")]
    public string Role { get; set; }
    [BsonElement("userSettings")]
    public ObjectId UserSettings { get; set; }
}

public class UserSettings
{
    [BsonElement("theme")]
    public string Theme { get; set; }
    [BsonElement("language")]
    public string Language { get; set; }
    [BsonElement("_id")]
    public ObjectId Id { get; set; }
    [BsonElement("user")]
    public ObjectId User { get; set; }
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    [BsonElement("__v")]
    public int V { get; set; }
}

public class UserWithStringId
{
    public string Username { get; set; }
    public string Email { get; set; }
    [JsonPropertyName("_id")]
    public string Id { get; set; }
    public string ProfilePicture { get; set; }
    public string Role { get; set; }
}