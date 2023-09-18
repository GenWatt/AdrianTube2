using MongoDB.Driver;
using AdrianTube2.Models.Movie;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using MongoDB.Bson;
using AdrianTube2.Models.UserModels;
using AdrianTube2.Services.Interfaces;

namespace AdrianTube2.Services;

public enum SubscriptionResult {
    Subscribed,
    UnSubscribed
}

public class SubscriptionAction {
    public SubscriptionResult Result { get; set; }
    public Subscribtion Subscribtion { get; set; }
}

public class SubscriptionService : ISubscriptionService
{
    private AuthenticationStateProvider _authenticationStateProvider { get; set; }
    readonly MongoClient _client = new MongoClient(Environment.GetEnvironmentVariable("MONGO_URI"));
    readonly IMongoCollection<Subscribtion> _subscriptions;
    readonly IMongoCollection<User> _usersCollection;

    public SubscriptionService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
        var adrianTubeDatabase = _client.GetDatabase("AdrianTube");
        var userDb = _client.GetDatabase("AdrianAuth");
        _subscriptions = adrianTubeDatabase.GetCollection<Subscribtion>("Subscriptions");
        _usersCollection = userDb.GetCollection<User>("users");
    }

    public async Task<List<Subscribtion>> GetSubscriptions()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var subscriptions = await _subscriptions.Find(c => c.User.Id == new ObjectId(userId)).ToListAsync();

        return subscriptions;
    }

    public async Task<SubscriptionAction> SubscribeOrUnSubscribe(User userToSubscribe) {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null) {
            var subscriber = await _subscriptions
                .Find(s => s.Creator.Id == userToSubscribe.Id && s.User.Id == new ObjectId(userId))
                .FirstOrDefaultAsync();

            if (subscriber != null) {
                await _subscriptions.DeleteOneAsync(s => s.Creator.Id == userToSubscribe.Id && s.User.Id == subscriber.User.Id);
                return new SubscriptionAction {
                    Result = SubscriptionResult.UnSubscribed,
                    Subscribtion = subscriber
                };
            } else {
                var dbUser = await _usersCollection.Find(u => u.Id == new ObjectId(userId)).FirstOrDefaultAsync();

                if (dbUser == null) throw new Exception("User to subscribe not found!");

                var subscription = new Subscribtion {
                    Creator = userToSubscribe,
                    User = dbUser
                };

                await _subscriptions.InsertOneAsync(subscription);
                return new SubscriptionAction {
                    Result = SubscriptionResult.Subscribed,
                    Subscribtion = subscription
                };
            }
        }
        throw new Exception("User not logged in!");
    }

    public async Task<int> GetSubscriptionCount(ObjectId id) {
        var subscriptionCount = await _subscriptions.Find(s => s.Creator.Id == id).CountDocumentsAsync();

        return (int)subscriptionCount;
    }

    public async Task<bool> IsSubscribing(ObjectId creatorId) {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null) {
            var isSubscribed = await _subscriptions.Find(s => s.Creator.Id == creatorId && s.User.Id == new ObjectId(userId)).AnyAsync();

            return isSubscribed;
        }

        return false;
    }
}
