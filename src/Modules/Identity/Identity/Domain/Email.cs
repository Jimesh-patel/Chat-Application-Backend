using Platform.Common.Results;
using System.Text.RegularExpressions;

namespace Identity.Domain;

public sealed record Email
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Email>.Failure(
                UserErrors.InvalidEmail);
        }

        value = value.Trim().ToLowerInvariant();

        if (!Regex.IsMatch(
                value,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return Result<Email>.Failure(
                UserErrors.InvalidEmail);
        }

        return Result<Email>.Success(
            new Email(value));
    }

    public override string ToString() => Value;
}