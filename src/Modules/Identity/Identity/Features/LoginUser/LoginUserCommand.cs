using Platform.Contracts.Commands;

namespace Identity.Features.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password) : ICommand<LoginUserResult>;

public sealed record LoginUserResult(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string Username,
    string Email);

public sealed record LoginUserResponse(
    string AccessToken,
    Guid UserId,
    string Username,
    string Email);
