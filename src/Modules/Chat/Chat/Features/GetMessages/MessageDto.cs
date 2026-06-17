using Chat.Domain;

namespace Chat.Features.GetMessages;

public sealed record MessageDto(
    Guid MessageId,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    MessageStatus Status,
    DateTime SentAtUtc,
    DateTime? SeenAtUtc);
