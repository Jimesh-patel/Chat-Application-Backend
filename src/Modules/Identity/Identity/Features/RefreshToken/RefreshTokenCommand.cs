using Platform.Contracts.Commands;
using Identity.Features.LoginUser;

namespace Identity.Features.RefreshToken;

public sealed record RefreshTokenCommand(
    Guid UserId,
    string RefreshToken) : ICommand<LoginUserResult>;
