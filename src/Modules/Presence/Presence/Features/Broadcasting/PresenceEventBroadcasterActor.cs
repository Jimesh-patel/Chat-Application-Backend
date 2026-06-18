using Akka.Actor;
using Platform.Realtime.Abstractions;
using Presence.Contracts.Events;
using Presence.Contracts.Responses;
using Presence.Infrastructure;
using Serilog;

namespace Presence.Features.Broadcasting;

public sealed class PresenceEventBroadcasterActor : ReceiveActor
{
    private readonly IPresenceSubscriptionManager _subscriptionManager;
    private readonly IRealtimeNotifier _realtimeNotifier;

    public PresenceEventBroadcasterActor(
        IPresenceSubscriptionManager subscriptionManager,
        IRealtimeNotifier realtimeNotifier)
    {
        _subscriptionManager = subscriptionManager;
        _realtimeNotifier = realtimeNotifier;

        ReceiveAsync<UserOnline>(HandleUserOnline);
        ReceiveAsync<UserOffline>(HandleUserOffline);
    }

    protected override void PreStart()
    {
        Context.System.EventStream.Subscribe(Self, typeof(UserOnline));
        Context.System.EventStream.Subscribe(Self, typeof(UserOffline));
        
        Log.Information("PresenceEventBroadcasterActor started and subscribed to presence events.");
        base.PreStart();
    }

    protected override void PostStop()
    {
        Context.System.EventStream.Unsubscribe(Self);
        Log.Information("PresenceEventBroadcasterActor stopped.");
        base.PostStop();
    }

    private async Task HandleUserOnline(UserOnline message)
    {
        var watchers = _subscriptionManager.GetWatchers(message.UserId).ToList();
        
        if (watchers.Count > 0)
        {
            await _realtimeNotifier.SendToConnectionsAsync(
                watchers,
                "PresenceOnline",
                new PresenceOnlineResponse(message.UserId));
        }
    }

    private async Task HandleUserOffline(UserOffline message)
    {
        var watchers = _subscriptionManager.GetWatchers(message.UserId).ToList();
        
        if (watchers.Count > 0)
        {
            await _realtimeNotifier.SendToConnectionsAsync(
                watchers,
                "PresenceOffline",
                new PresenceOfflineResponse(message.UserId, message.LastSeenAtUtc));
        }
    }
}
