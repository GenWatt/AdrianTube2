using AdrianTube2.Models.Movie;
using MongoDB.Bson;

namespace AdrianTube2.Services.Interfaces;

public interface ISubscriptionService
{
    Task<List<Subscribtion>> GetSubscriptions();
    Task<SubscriptionResult> SubscribeOrUnSubscribe(Movie movie);
    Task<bool> IsSubscribing(ObjectId creatorId);
    Task<int> GetSubscriptionCount(ObjectId id);
}