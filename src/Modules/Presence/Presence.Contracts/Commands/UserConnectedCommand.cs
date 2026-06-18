using Platform.Contracts.Commands;

namespace Presence.Contracts.Commands;

public sealed record UserConnectedCommand(
    Guid UserId,
    string ConnectionId) : ICommand<Guid>;
