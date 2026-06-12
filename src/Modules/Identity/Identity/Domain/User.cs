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

    private List<RefreshToken> _refreshTokens = [];

    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

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

    public void Login(string refreshTokenHash, DateTime refreshTokenExpiry)
    {
        Raise(new UserLoggedIn(
            Guid.NewGuid(),
            Id,
            refreshTokenHash,
            refreshTokenExpiry,
            DateTime.UtcNow));
    }

    public void Logout(string refreshTokenHash)
    {
        Raise(new UserLoggedOut(
            Guid.NewGuid(),
            Id,
            refreshTokenHash,
            DateTime.UtcNow));
    }

    public void Apply(UserRegistered e)
    {
        Id = e.UserId;
        Email = Email.Create(e.Email).Value;
        Username = Username.Create(e.Username).Value;
        PasswordHash = PasswordHash.FromHash(e.PasswordHash);
        CreatedAtUtc = e.OccurredOnUtc;
    }

    public void Apply(UserLoggedIn e)
    {
        _refreshTokens ??= [];
        _refreshTokens.Add(new RefreshToken(e.RefreshTokenHash, e.RefreshTokenExpiryUtc));
    }

    private void Apply(UserLoggedOut e)
    {
        _refreshTokens.RemoveAll(rt => rt.TokenHash == e.RefreshTokenHash);
    }
}