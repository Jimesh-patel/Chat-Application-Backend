using Platform.Contracts.Commands;

namespace Identity.Features.LogoutUser;

public sealed record LogoutUserCommand(
    Guid UserId,
    string RefreshToken) : ICommand;
