using Platform.Common.Results;

namespace Identity.Domain;

public static class UserErrors
{
    public static readonly Error InvalidEmail =
        new(
            "Identity.InvalidEmail",
            "Email address is invalid.");

    public static readonly Error InvalidUsername =
        new(
            "Identity.InvalidUsername",
            "Username is invalid.");

    public static readonly Error InvalidPassword =
        new(
            "Identity.InvalidPassword",
            "Password is invalid.");
}