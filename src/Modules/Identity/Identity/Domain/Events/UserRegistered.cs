using Platform.Common.Events;

namespace Identity.Domain.Events;

public sealed record UserRegistered(
    UserId UserId,
    string Email,
    string Username,
    string DisplayName,
    string PasswordHash,
    DateTime CreatedAtUtc)
    : DomainEvent;