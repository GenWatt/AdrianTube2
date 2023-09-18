using AdrianTube2.Models.Movie;
using AdrianTube2.Models.UserModels;
using MongoDB.Bson;

namespace AdrianTube2.Services.Interfaces;

public interface ISubscriptionService
{
    Task<List<Subscribtion>> GetSubscriptions();
    Task<SubscriptionAction> SubscribeOrUnSubscribe(User movie);
    Task<bool> IsSubscribing(ObjectId creatorId);
    Task<int> GetSubscriptionCount(ObjectId id);
}