using Platform.Common.Entities;
using Platform.Common.Events;

namespace Identity.Domain.Events;

public sealed record UserLoggedOut(
    Guid EventId,
    UserId UserId,
    string RefreshTokenHash,
    DateTime OccurredOnUtc) : IDomainEvent;
