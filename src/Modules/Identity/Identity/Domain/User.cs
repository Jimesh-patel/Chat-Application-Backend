using Identity.Domain.Events;
using Platform.Common.Entities;

namespace Identity.Domain;

public sealed class User : AggregateRoot<UserId>
{
    private User() {}

    public Email Email { get; private set; } = null!;

    public Username Username { get; private set; } = null!;

    public PasswordHash PasswordHash { get; private set; } = null!;

    public DateTime CreatedAtUtc { get; private set; }

    public static User Register(
        UserId id,
        Email email,
        Username username,
        PasswordHash passwordHash)
    {
        var user = new User
        {
            Id = id,
            Email = email,
            Username = username,
            PasswordHash = passwordHash,
            CreatedAtUtc = DateTime.UtcNow
        };

        user.Raise(
    new UserRegistered(
                id,
                email.Value,
                username.Value,
                username.Value,
                passwordHash.Value,
                user.CreatedAtUtc));

        return user;
    }
}