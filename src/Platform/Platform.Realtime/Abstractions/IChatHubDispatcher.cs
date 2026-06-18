namespace Platform.Realtime.Abstractions;

public interface IChatHubDispatcher
{
    Task MarkMessageSeenAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken = default);
    Task<object> SendMessageAsync(Guid senderId, Guid conversationId, Guid recipientId, string content, CancellationToken cancellationToken = default);
    Task SetUserTypingAsync(Guid conversationId, Guid userId, bool isTyping, CancellationToken cancellationToken = default);
    Task UserConnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default);
    Task UserDisconnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default);
    Task SubscribeToPresenceAsync(Guid watcherUserId, string connectionId, Guid targetUserId, CancellationToken cancellationToken = default);
    Task UnsubscribeFromPresenceAsync(string connectionId, Guid targetUserId, CancellationToken cancellationToken = default);
}
