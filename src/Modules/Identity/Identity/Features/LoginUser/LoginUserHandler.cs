using Identity.Domain;
using Identity.ReadModels;
using Marten;
using Microsoft.Extensions.Options;
using Platform.Auth;
using Platform.Common.Results;
using Platform.Contracts;
using Platform.Contracts.Commands;

namespace Identity.Features.LoginUser;

internal sealed class LoginUserHandler(
    IDocumentSession session,
    IQuerySession querySession,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtOptions> jwtOptions)
    : ICommandHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<Result<LoginUserResponse>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        Serilog.Log.Information("Handling LoginUserCommand for {Email}", command.Email);

        var userReadModel = await querySession
            .Query<UserReadModel>()
            .FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken);

        if (userReadModel is null)
        {
            return Result<LoginUserResponse>.Failure(new Error("Identity.InvalidCredentials", "Invalid email or password."));
        }

        if (!passwordHasher.Verify(command.Password, userReadModel.PasswordHash))
        {
            return Result<LoginUserResponse>.Failure(new Error("Identity.InvalidCredentials", "Invalid email or password."));
        }

        var accessToken = jwtTokenGenerator.GenerateToken(userReadModel.Id, userReadModel.Email, userReadModel.Username);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = passwordHasher.Hash(refreshToken);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationInDays);

        var userAggregate = await session.Events.AggregateStreamAsync<User>(userReadModel.Id, token: cancellationToken);
        if (userAggregate is null)
        {
            return Result<LoginUserResponse>.Failure(new Error("Identity.UserNotFound", "User aggregate not found."));
        }

        userAggregate.Login(refreshTokenHash, refreshTokenExpiry);
        session.Events.Append(userAggregate.Id.Value, userAggregate.DomainEvents.ToArray());
        await session.SaveChangesAsync(cancellationToken);

        return Result<LoginUserResponse>.Success(new LoginUserResponse(accessToken, refreshToken));
    }
}
