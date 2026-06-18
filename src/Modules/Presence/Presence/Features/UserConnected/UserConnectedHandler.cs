using Akka.Actor;
using Akka.Hosting;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using Presence.Contracts.Commands;
using Presence.Infrastructure.Actors;
using Serilog;

namespace Presence.Features.UserConnected;

internal sealed class UserConnectedHandler(IActorRegistry actorRegistry) 
    : ICommandHandler<UserConnectedCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UserConnectedCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var manager = actorRegistry.Get<UserPresenceManagerActor>();
            
            var actor = await manager.Ask<IActorRef>(
                new GetUserPresenceActor(command.UserId), 
                TimeSpan.FromSeconds(5), 
                cancellationToken);

            actor.Tell(new UserConnectedActorCommand(command.ConnectionId));
            
            return Result<Guid>.Success(command.UserId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed handling UserConnected for user {UserId}", command.UserId);
            return Result<Guid>.Failure(new Error("Presence.UserConnectedFailed", "Failed to process user connected event."));
        }
    }
}
