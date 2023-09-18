using AdrianTube2.Models.Movie;

namespace AdrianTube2.state.Interfaces;

public interface ISubscriptionState
{
    List<Subscribtion> Subscriptions { get; set; }
    void AddSubscription(Subscribtion subscription);
    void RemoveSubscription(Subscribtion subscription);
    void ClearSubscriptions();
    void AddSubscriptions(List<Subscribtion> subscriptions);
    event Action? OnChange;
}