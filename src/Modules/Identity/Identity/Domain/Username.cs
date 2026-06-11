using Platform.Common.Results;

namespace Identity.Domain;

public sealed record Username
{
    public string Value { get; }

    private Username(string value)
    {
        Value = value;
    }

    public static Result<Username> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Username>.Failure(
                UserErrors.InvalidUsername);
        }

        value = value.Trim();

        if (value.Length < 3)
        {
            return Result<Username>.Failure(
                UserErrors.InvalidUsername);
        }

        return Result<Username>.Success(
            new Username(value));
    }

    public override string ToString() => Value;
}