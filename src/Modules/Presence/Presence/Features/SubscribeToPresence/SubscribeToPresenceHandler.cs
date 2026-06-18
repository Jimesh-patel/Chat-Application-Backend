using Akka.Actor;
using Akka.Hosting;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using Platform.Realtime.Abstractions;
using Presence.Contracts.Commands;
using Presence.Contracts.Responses;
using Presence.Domain;
using Presence.Infrastructure;
using Presence.Infrastructure.Actors;
using Serilog;

namespace Presence.Features.SubscribeToPresence;

internal sealed class SubscribeToPresenceHandler(
    IActorRegistry actorRegistry,
    IPresenceSubscriptionManager subscriptionManager,
    IRealtimeNotifier realtimeNotifier) 
    : ICommandHandler<SubscribeToPresenceCommand, Guid>
{
    public async Task<Result<Guid>> Handle(SubscribeToPresenceCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Add subscription
            subscriptionManager.AddSubscription(command.TargetUserId, command.ConnectionId);

            // 2. Fetch current status
            var manager = actorRegistry.Get<UserPresenceManagerActor>();
            var actor = await manager.Ask<IActorRef>(
                new GetUserPresenceActor(command.TargetUserId), 
                TimeSpan.FromSeconds(5), 
                cancellationToken);

            var state = await actor.Ask<PresenceState>(
                new GetUserPresenceActorCommand(command.TargetUserId), 
                TimeSpan.FromSeconds(5), 
                cancellationToken);

            // 3. Send initial presence state to the connection
            if (state.IsOnline)
            {
                await realtimeNotifier.SendToConnectionsAsync(
                    [command.ConnectionId],
                    "PresenceOnline",
                    new PresenceOnlineResponse(command.TargetUserId),
                    cancellationToken);
            }
            else
            {
                await realtimeNotifier.SendToConnectionsAsync(
                    [command.ConnectionId],
                    "PresenceOffline",
                    new PresenceOfflineResponse(command.TargetUserId, state.LastSeenAtUtc ?? DateTime.UtcNow),
                    cancellationToken);
            }

            return Result<Guid>.Success(command.TargetUserId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed handling SubscribeToPresence for watcher {Watcher} to target {Target}", command.WatcherUserId, command.TargetUserId);
            return Result<Guid>.Failure(new Error("Presence.SubscribeFailed", "Failed to subscribe to presence."));
        }
    }
}
