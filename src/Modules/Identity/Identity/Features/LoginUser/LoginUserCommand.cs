using Platform.Contracts.Commands;

namespace Identity.Features.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password) : ICommand<LoginUserResult>;

public sealed record LoginUserResult(
    string AccessToken,
    string RefreshToken);

public sealed record LoginUserResponse(
    string AccessToken);
