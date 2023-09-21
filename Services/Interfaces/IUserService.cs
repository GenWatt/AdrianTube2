
using AdrianTube2.Models.UserModels;
using MongoDB.Bson;

namespace AdrianTube2.Services.Interfaces;

public interface IUserService
{
    public Task<User?> GetUser(ObjectId id);
}