namespace Chat.Features.UserTyping;

public sealed record SetUserTypingActorCommand(
    Guid ConversationId,
    Guid UserId,
    bool IsTyping);
