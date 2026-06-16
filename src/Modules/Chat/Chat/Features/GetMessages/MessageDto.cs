namespace Chat.Features.GetMessages;

public sealed record MessageDto(
    Guid MessageId,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    DateTime SentAtUtc);
