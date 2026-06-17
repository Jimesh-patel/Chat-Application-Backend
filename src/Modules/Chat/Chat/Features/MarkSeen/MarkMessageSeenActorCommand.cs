using Chat.Domain;

namespace Chat.Features.MarkSeen;

internal sealed record MarkMessageSeenActorCommand(
    ConversationId ConversationId,
    MessageId MessageId);
