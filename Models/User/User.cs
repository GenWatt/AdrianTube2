using System.Text.Json.Serialization;

namespace AdrianTube2.Models.User;

public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    [JsonPropertyName("_id")]
    public string Id { get; set; }
}