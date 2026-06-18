using Platform.Contracts.Commands;

namespace Presence.Contracts.Commands;

public sealed record SubscribeToPresenceCommand(
    Guid WatcherUserId,
    string ConnectionId,
    Guid TargetUserId) : ICommand<Guid>;
