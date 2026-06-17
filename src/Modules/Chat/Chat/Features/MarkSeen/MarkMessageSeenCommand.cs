using Platform.Contracts.Commands;

namespace Chat.Features.MarkSeen;

public sealed record MarkMessageSeenCommand(
    Guid ConversationId,
    Guid MessageId) : ICommand<Guid>;
