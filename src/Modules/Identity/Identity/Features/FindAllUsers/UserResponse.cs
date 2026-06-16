namespace Identity.Features.FindAllUsers;

/// <summary>
/// Response DTO for user information.
/// </summary>
public sealed record UserResponse(
    Guid Id,
    string Email,
    string Username,
    string DisplayName);
