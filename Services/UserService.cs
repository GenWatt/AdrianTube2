using AdrianTube2.Models.UserModels;
using AdrianTube2.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AdrianTube2.Services;

public class UserService : IUserService
{
    private readonly MongoClient _client = new(Environment.GetEnvironmentVariable("MONGO_URI")!);
    private IMongoCollection<User> _userCollection { get; set; }

    public UserService()
    {
        _userCollection = _client.GetDatabase("AdrianAuth").GetCollection<User>("users");
    }

    public async Task<User?> GetUser(ObjectId id)
    {
        var user = await _userCollection.Find(user => user.Id == id).FirstOrDefaultAsync();

        return user;
    }
}