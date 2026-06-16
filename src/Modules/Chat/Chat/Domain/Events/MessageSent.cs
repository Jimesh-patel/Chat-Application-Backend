using Platform.Common.Events;

namespace Chat.Domain.Events;

/// <summary>
/// Domain event raised when a message is sent within a conversation.
/// This is the source-of-truth event stored in the Marten event stream.
/// </summary>
public sealed record MessageSent(
    ConversationId ConversationId,
    MessageId MessageId,
    Guid SenderId,
    Guid RecipientId,
    string Content,
    DateTime SentAtUtc) : DomainEvent;
