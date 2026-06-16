using Platform.Common.Events;

namespace Chat.Domain.Events;

/// <summary>
/// Domain event raised when a new conversation is started.
/// </summary>
public sealed record ConversationStarted(
    ConversationId ConversationId,
    Guid ParticipantA,
    Guid ParticipantB,
    DateTime StartedAtUtc) : DomainEvent;
