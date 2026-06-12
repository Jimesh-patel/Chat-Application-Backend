using Platform.Common.Entities;
using Platform.Common.Events;

namespace Identity.Domain.Events;

public sealed record UserLoggedIn(
    Guid EventId,
    UserId UserId,
    string RefreshTokenHash,
    DateTime RefreshTokenExpiryUtc,
    DateTime OccurredOnUtc) : IDomainEvent;
