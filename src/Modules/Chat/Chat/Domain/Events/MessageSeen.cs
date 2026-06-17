using Platform.Common.Events;

namespace Chat.Domain.Events;

public sealed record MessageSeen(
    MessageId MessageId,
    DateTime SeenAtUtc) : DomainEvent;
