using Identity.Domain;
using Identity.ReadModels;
using Marten;
using Platform.Common.Results;
using Platform.Contracts.Commands;

namespace Identity.Features.RegisterUser;

internal sealed class RegisterUserHandler(
    IDocumentSession session,
    IQuerySession querySession)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var emailResult = Email.Create(command.Email);

        if (emailResult.IsFailure)
        {
            return Result<Guid>.Failure(
                emailResult.Error!);
        }

        var usernameResult = Username.Create(
            command.Username);

        if (usernameResult.IsFailure)
        {
            return Result<Guid>.Failure(
                usernameResult.Error!);
        }

        var email = emailResult.Value!;
        var username = usernameResult.Value!;

        var emailExists = await querySession
            .Query<UserReadModel>()
            .AnyAsync(
                x => x.Email == email.Value,
                cancellationToken);

        if (emailExists)
        {
            return Result<Guid>.Failure(
                new Error(
                    "Identity.EmailAlreadyExists",
                    "Email already exists."));
        }

        var userId = new UserId(Guid.NewGuid());

        var user = User.Register(
            userId,
            email,
            username,
            new PasswordHash(command.Password));

        session.Events.StartStream(
            userId.Value,
            user.DomainEvents);

        await session.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            userId.Value);
    }
}