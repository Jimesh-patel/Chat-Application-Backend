using Platform.Contracts.Queries;

namespace Chat.Features.FindAllConversations;

/// <summary>
/// Query to find all conversations for a specific user.
/// </summary>
public sealed record FindAllConversationsQuery(Guid UserId) : IQuery<IReadOnlyList<ConversationResponse>>;
