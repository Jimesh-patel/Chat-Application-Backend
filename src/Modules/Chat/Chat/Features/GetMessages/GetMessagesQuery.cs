using Platform.Contracts.Queries;

namespace Chat.Features.GetMessages;

/// <summary>
/// Query to retrieve all messages for a specific conversation.
/// </summary>
public sealed record GetMessagesQuery(
    Guid ConversationId) : IQuery<IReadOnlyList<MessageDto>>;
