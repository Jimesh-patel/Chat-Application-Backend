using Platform.Contracts.Commands;

namespace Presence.Contracts.Commands;

public sealed record UserDisconnectedCommand(
    Guid UserId,
    string ConnectionId) : ICommand<Guid>;
