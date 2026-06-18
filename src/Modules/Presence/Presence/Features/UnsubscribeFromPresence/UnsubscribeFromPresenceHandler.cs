using Platform.Common.Results;
using Platform.Contracts.Commands;
using Presence.Contracts.Commands;
using Presence.Infrastructure;
using Serilog;

namespace Presence.Features.UnsubscribeFromPresence;

internal sealed class UnsubscribeFromPresenceHandler(
    IPresenceSubscriptionManager subscriptionManager) 
    : ICommandHandler<UnsubscribeFromPresenceCommand, Guid>
{
    public Task<Result<Guid>> Handle(UnsubscribeFromPresenceCommand command, CancellationToken cancellationToken)
    {
        try
        {
            subscriptionManager.RemoveSubscription(command.TargetUserId, command.ConnectionId);
            return Task.FromResult(Result<Guid>.Success(command.TargetUserId));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed handling UnsubscribeFromPresence for connection {Connection} to target {Target}", command.ConnectionId, command.TargetUserId);
            return Task.FromResult(Result<Guid>.Failure(new Error("Presence.UnsubscribeFailed", "Failed to unsubscribe from presence.")));
        }
    }
}
