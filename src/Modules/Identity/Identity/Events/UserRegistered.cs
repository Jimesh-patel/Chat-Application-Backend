using Identity.Domain;
using Platform.Common.Events;

namespace Identity.Events;

public sealed record UserRegistered(
    UserId UserId,
    string Email)
    : DomainEvent;
