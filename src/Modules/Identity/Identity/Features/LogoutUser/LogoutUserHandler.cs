using Identity.Domain;
using Marten;
using Platform.Auth;
using Platform.Common.Results;
using Platform.Contracts.Commands;

namespace Identity.Features.LogoutUser;

internal sealed class LogoutUserHandler(
    IDocumentSession session,
    IPasswordHasher passwordHasher)
    : ICommandHandler<LogoutUserCommand>
{
    public async Task<Result> Handle(
        LogoutUserCommand command,
        CancellationToken cancellationToken)
    {
        var userAggregate = await session.Events.AggregateStreamAsync<User>(command.UserId, token: cancellationToken);
        if (userAggregate is null)
        {
            return Result.Failure(new Error("Identity.UserNotFound", "User not found."));
        }

        var refreshTokenHash = passwordHasher.Hash(command.RefreshToken);

        userAggregate.Logout(refreshTokenHash);

        session.Events.Append(command.UserId, [.. userAggregate.DomainEvents]);
        await session.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
