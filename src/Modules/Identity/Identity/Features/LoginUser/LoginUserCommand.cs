using Platform.Contracts.Commands;

namespace Identity.Features.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password) : ICommand<LoginUserResponse>;

public sealed record LoginUserResponse(
    string AccessToken,
    string RefreshToken);
