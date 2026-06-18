namespace Presence.Contracts.Events;

public sealed record UserOffline(
    Guid UserId,
    DateTime LastSeenAtUtc);
