using Platform.Common.Results;
using Platform.Contracts;

namespace Identity.Domain;

public sealed record PasswordHash
{
    public string Value { get; init; }

    private PasswordHash(string value) => Value = value;

    public static Result<PasswordHash> Create(string password, IPasswordHasher passwordHasher)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Result<PasswordHash>.Failure(new Error("Password.Empty", "Password cannot be empty."));
        }

        if (password.Length < 8)
        {
            return Result<PasswordHash>.Failure(new Error("Password.TooShort", "Password must be at least 8 characters long."));
        }

        return Result<PasswordHash>.Success(new PasswordHash(passwordHasher.Hash(password)));
    }
}