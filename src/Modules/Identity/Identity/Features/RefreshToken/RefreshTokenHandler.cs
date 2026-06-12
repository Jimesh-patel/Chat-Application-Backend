using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity.Domain;
using Identity.Features.LoginUser;
using Marten;
using Microsoft.Extensions.Options;
using Platform.Auth;
using Platform.Common.Results;
using Platform.Contracts;
using Platform.Contracts.Commands;

namespace Identity.Features.RefreshToken;

internal sealed class RefreshTokenHandler(
    IDocumentSession session,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtOptions> jwtOptions)
    : ICommandHandler<RefreshTokenCommand, LoginUserResult>
{
    public async Task<Result<LoginUserResult>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var userId = command.UserId;

        var userAggregate = await session.Events.AggregateStreamAsync<User>(userId, token: cancellationToken);
        if (userAggregate is null)
        {
            return Result<LoginUserResult>.Failure(new Error("Identity.UserNotFound", "User not found."));
        }

        var validToken = userAggregate.RefreshTokens.FirstOrDefault(rt => 
            passwordHasher.Verify(command.RefreshToken, rt.TokenHash) && rt.ExpiryUtc > DateTime.UtcNow);

        if (validToken is null)
        {
            return Result<LoginUserResult>.Failure(new Error("Identity.InvalidRefreshToken", "Invalid or expired refresh token."));
        }

        var email = userAggregate.Email.Value;
        var username = userAggregate.Username.Value;

        var newAccessToken = jwtTokenGenerator.GenerateToken(userId, email, username);
        var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshTokenHash = passwordHasher.Hash(newRefreshToken);
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationInDays);

        userAggregate.Login(newRefreshTokenHash, newRefreshTokenExpiry);

        session.Events.Append(userId, [.. userAggregate.DomainEvents]);
        await session.SaveChangesAsync(cancellationToken);

        return Result<LoginUserResult>.Success(new LoginUserResult(newAccessToken, newRefreshToken));
    }
}
