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
    : ICommandHandler<RefreshTokenCommand, LoginUserResponse>
{
    public async Task<Result<LoginUserResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(command.AccessToken))
        {
            return Result<LoginUserResponse>.Failure(new Error("Identity.InvalidToken", "Invalid access token."));
        }

        var jwtToken = handler.ReadJwtToken(command.AccessToken);
        var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Result<LoginUserResponse>.Failure(new Error("Identity.InvalidToken", "Invalid access token subject."));
        }

        var userAggregate = await session.Events.AggregateStreamAsync<User>(userId, token: cancellationToken);
        if (userAggregate is null)
        {
            return Result<LoginUserResponse>.Failure(new Error("Identity.UserNotFound", "User not found."));
        }

        var validToken = userAggregate.RefreshTokens.FirstOrDefault(rt => 
            passwordHasher.Verify(command.RefreshToken, rt.TokenHash) && rt.ExpiryUtc > DateTime.UtcNow);

        if (validToken is null)
        {
            return Result<LoginUserResponse>.Failure(new Error("Identity.InvalidRefreshToken", "Invalid or expired refresh token."));
        }

        var email = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value ?? string.Empty;
        var username = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value ?? string.Empty;

        var newAccessToken = jwtTokenGenerator.GenerateToken(userId, email, username);
        var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshTokenHash = passwordHasher.Hash(newRefreshToken);
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationInDays);

        userAggregate.Login(newRefreshTokenHash, newRefreshTokenExpiry);

        session.Events.Append(userAggregate.Id.Value, userAggregate.DomainEvents.ToArray());
        await session.SaveChangesAsync(cancellationToken);

        return Result<LoginUserResponse>.Success(new LoginUserResponse(newAccessToken, newRefreshToken));
    }
}
