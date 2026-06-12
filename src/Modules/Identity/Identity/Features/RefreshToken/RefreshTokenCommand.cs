using Platform.Contracts.Commands;
using Identity.Features.LoginUser;

namespace Identity.Features.RefreshToken;

public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken) : ICommand<LoginUserResponse>;
