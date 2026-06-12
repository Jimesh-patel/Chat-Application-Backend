using Identity.Domain;
using Identity.ReadModels;
using Marten;
using Microsoft.Extensions.Options;
using Platform.Auth;
using Platform.Common.Results;
using Platform.Contracts;
using Platform.Contracts.Commands;
using Serilog;

namespace Identity.Features.LoginUser;

internal sealed class LoginUserHandler(
    IDocumentSession session,
    IQuerySession querySession,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtOptions> jwtOptions)
    : ICommandHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<Result<LoginUserResult>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        Log.Information("Handling LoginUserCommand for {Email}", command.Email);

        var userReadModel = await querySession
            .Query<UserReadModel>()
            .FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken);

        if (userReadModel is null)
        {
            return Result<LoginUserResult>.Failure(new Error("Identity.InvalidCredentials", "Invalid email or password."));
        }

        if (!passwordHasher.Verify(command.Password, userReadModel.PasswordHash))
        {
            return Result<LoginUserResult>.Failure(new Error("Identity.InvalidCredentials", "Invalid email or password."));
        }

        var accessToken = jwtTokenGenerator.GenerateToken(userReadModel.Id, userReadModel.Email, userReadModel.Username);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = passwordHasher.Hash(refreshToken);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpirationInDays);

        var userAggregate = await session.Events.AggregateStreamAsync<User>(userReadModel.Id.Value, token: cancellationToken);
        if (userAggregate is null)
        {
            return Result<LoginUserResult>.Failure(new Error("Identity.UserNotFound", "User aggregate not found."));
        }

        userAggregate.Login(refreshTokenHash, refreshTokenExpiry);
        session.Events.Append(userReadModel.Id.Value, userAggregate.DomainEvents.ToArray());
        await session.SaveChangesAsync(cancellationToken);

        return Result<LoginUserResult>.Success(new LoginUserResult(accessToken, refreshToken));
    }
}
