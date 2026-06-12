using Identity.Domain;
using Identity.ReadModels;
using Marten;
using Platform.Auth;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using Serilog;

namespace Identity.Features.RegisterUser;

internal sealed class RegisterUserHandler(
    IDocumentSession session,
    IQuerySession querySession,
    IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        Log.Information("Handling RegisterUserCommand for {Email}", command.Email);

        var emailResult = Email.Create(command.Email);

        if (emailResult.IsFailure)
        {
            Log.Warning("Failed to create email: {Error}", emailResult.Error);
            return Result<Guid>.Failure(
                emailResult.Error!);
        }

        var usernameResult = Username.Create(
            command.Username);

        if (usernameResult.IsFailure)
        {
            Log.Warning("Failed to create username: {Error}", usernameResult.Error);
            return Result<Guid>.Failure(
                usernameResult.Error!);
        }

        var passwordResult = PasswordHash.Create(command.Password, passwordHasher);

        if (passwordResult.IsFailure)
        {
            Log.Warning("Failed to create password hash: {Error}", passwordResult.Error);
            return Result<Guid>.Failure(
                passwordResult.Error!);
        }

        var email = emailResult.Value!;
        var username = usernameResult.Value!;
        var passwordHash = passwordResult.Value!;

        try
        {
            var emailExists = await querySession
                .Query<UserReadModel>()
                .AnyAsync(
                    x => x.Email == email.Value,
                    cancellationToken);

            if (emailExists)
            {
                Log.Warning("Registration failed: Email {Email} already exists", email.Value);
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
                passwordHash);

            session.Events.StartStream(
                userId.Value,
                user.DomainEvents);

            await session.SaveChangesAsync(cancellationToken);

            Log.Information("Successfully registered user {UserId} with email {Email}", userId.Value, email.Value);

            return Result<Guid>.Success(userId.Value);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred during user registration for {Email}", command.Email);
            return Result<Guid>.Failure(
                new Error("Identity.RegistrationFailed", "An unexpected error occurred during registration."));
        }
    }
}