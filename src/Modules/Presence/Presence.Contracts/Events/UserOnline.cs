namespace Presence.Contracts.Events;

public sealed record UserOnline(
    Guid UserId,
    DateTime OccurredAtUtc);
