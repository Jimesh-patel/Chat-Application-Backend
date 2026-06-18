using Platform.Contracts.Commands;

namespace Presence.Contracts.Commands;

public sealed record UnsubscribeFromPresenceCommand(
    string ConnectionId,
    Guid TargetUserId) : ICommand<Guid>;
