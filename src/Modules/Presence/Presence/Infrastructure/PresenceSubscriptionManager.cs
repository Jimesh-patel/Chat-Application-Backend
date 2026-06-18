using System.Collections.Concurrent;

namespace Presence.Infrastructure;

public interface IPresenceSubscriptionManager
{
    void AddSubscription(Guid targetUserId, string connectionId);
    void RemoveSubscription(Guid targetUserId, string connectionId);
    IEnumerable<string> GetWatchers(Guid targetUserId);
    void RemoveConnection(string connectionId);
}

public sealed class PresenceSubscriptionManager : IPresenceSubscriptionManager
{
    // Key: Target User Id, Value: Set of Watching Connection Ids
    private readonly ConcurrentDictionary<Guid, HashSet<string>> _subscriptions = new();

    public void AddSubscription(Guid targetUserId, string connectionId)
    {
        _subscriptions.AddOrUpdate(
            targetUserId,
            _ => [connectionId],
            (_, watchers) =>
            {
                lock (watchers)
                {
                    watchers.Add(connectionId);
                }
                return watchers;
            });
    }

    public void RemoveSubscription(Guid targetUserId, string connectionId)
    {
        if (_subscriptions.TryGetValue(targetUserId, out var watchers))
        {
            lock (watchers)
            {
                watchers.Remove(connectionId);
            }
        }
    }

    public IEnumerable<string> GetWatchers(Guid targetUserId)
    {
        if (_subscriptions.TryGetValue(targetUserId, out var watchers))
        {
            lock (watchers)
            {
                return watchers.ToList();
            }
        }
        return [];
    }

    public void RemoveConnection(string connectionId)
    {
        foreach (var kvp in _subscriptions)
        {
            lock (kvp.Value)
            {
                kvp.Value.Remove(connectionId);
            }
        }
    }
}
