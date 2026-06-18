namespace Presence.Contracts.Responses;

public sealed record PresenceOfflineResponse(
    Guid UserId,
    DateTime LastSeenAtUtc);
