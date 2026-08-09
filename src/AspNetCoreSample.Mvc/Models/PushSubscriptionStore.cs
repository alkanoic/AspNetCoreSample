using System.Collections.Concurrent;

namespace AspNetCoreSample.Mvc.Models;

public static class PushSubscriptionStore
{
    private static readonly ConcurrentDictionary<string, SubscribeViewModel> _subscriptions = new();

    public static void Set(string userId, SubscribeViewModel subscription)
    {
        _subscriptions[userId] = subscription;
    }

    public static SubscribeViewModel? Get(string userId)
    {
        _subscriptions.TryGetValue(userId, out var subscription);
        return subscription;
    }
}
