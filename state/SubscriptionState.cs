using AdrianTube2.Models.Movie;
using AdrianTube2.state.Interfaces;

namespace AdrianTube2.state;

public class SubscribtionState : ISubscriptionState
{
    List<Subscribtion> _subscriptions = new ();
    public event Action? OnChange;

    public List<Subscribtion> Subscriptions {
        get { return _subscriptions; }
        set {
            _subscriptions = value;
            NotifyStateChanged();
        }
    }

    public void AddSubscription(Subscribtion subscription)
    {
        _subscriptions.Add(subscription);
        NotifyStateChanged();
    }

    public void AddSubscriptions(List<Subscribtion> subscriptions)
    {
        _subscriptions.AddRange(subscriptions);
        NotifyStateChanged();
    }

    public void ClearSubscriptions()
    {
        _subscriptions.Clear();
        NotifyStateChanged();
    }

    public void RemoveSubscription(Subscribtion subscription)
    {
        var index = _subscriptions.FindIndex(s => s.Id == subscription.Id);
        _subscriptions.RemoveAt(index);
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}