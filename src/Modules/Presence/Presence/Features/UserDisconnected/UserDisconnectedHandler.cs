using Akka.Actor;
using Akka.Hosting;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using Presence.Contracts.Commands;
using Presence.Infrastructure;
using Presence.Infrastructure.Actors;
using Serilog;

namespace Presence.Features.UserDisconnected;

internal sealed class UserDisconnectedHandler(
    IActorRegistry actorRegistry,
    IPresenceSubscriptionManager subscriptionManager) 
    : ICommandHandler<UserDisconnectedCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UserDisconnectedCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Remove connection from all subscriptions
            subscriptionManager.RemoveConnection(command.ConnectionId);

            var manager = actorRegistry.Get<UserPresenceManagerActor>();
            
            var actor = await manager.Ask<IActorRef>(
                new GetUserPresenceActor(command.UserId), 
                TimeSpan.FromSeconds(5), 
                cancellationToken);

            actor.Tell(new UserDisconnectedActorCommand(command.ConnectionId));
            
            return Result<Guid>.Success(command.UserId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed handling UserDisconnected for user {UserId}", command.UserId);
            return Result<Guid>.Failure(new Error("Presence.UserDisconnectedFailed", "Failed to process user disconnected event."));
        }
    }
}
