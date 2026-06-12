using Identity.Domain;

namespace Identity.ReadModels;

public sealed class UserReadModel
{
    public UserId Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string PasswordHash {  get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}