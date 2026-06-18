using Chat.Features.GetMessages;
using Chat.Features.MarkSeen;
using Chat.Features.SendMessage;
using Chat.Features.UserTyping;
using Platform.Contracts.Commands;
using Platform.Realtime.Abstractions;

namespace Chat.Infrastructure.Realtime;

internal sealed class ChatHubDispatcher(
    ICommandHandler<SendMessageCommand, MessageDto> sendMessageHandler,
    ICommandHandler<MarkMessageSeenCommand, Guid> markSeenHandler,
    ICommandHandler<SetUserTypingCommand, Guid> setUserTypingHandler,
    ICommandHandler<Presence.Contracts.Commands.UserConnectedCommand, Guid> userConnectedHandler,
    ICommandHandler<Presence.Contracts.Commands.UserDisconnectedCommand, Guid> userDisconnectedHandler,
    ICommandHandler<Presence.Contracts.Commands.SubscribeToPresenceCommand, Guid> subscribeToPresenceHandler,
    ICommandHandler<Presence.Contracts.Commands.UnsubscribeFromPresenceCommand, Guid> unsubscribeFromPresenceHandler)
    : IChatHubDispatcher
{
    public async Task<object> SendMessageAsync(Guid senderId, Guid conversationId, Guid recipientId, string content, CancellationToken cancellationToken = default)
    {
        var command = new SendMessageCommand(recipientId, content)
        {
            ConversationId = conversationId,
            SenderId = senderId
        };
        
        var result = await sendMessageHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            throw new Exception(result.Error?.Description ?? "Failed to send message.");
        }

        return result.Value!;
    }

    public async Task MarkMessageSeenAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken = default)
    {
        await markSeenHandler.Handle(
            new MarkMessageSeenCommand(conversationId, messageId),
            cancellationToken);
    }

    public async Task SetUserTypingAsync(Guid conversationId, Guid userId, bool isTyping, CancellationToken cancellationToken = default)
    {
        await setUserTypingHandler.Handle(
            new SetUserTypingCommand(conversationId, userId, isTyping),
            cancellationToken);
    }

    public async Task UserConnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default)
    {
        await userConnectedHandler.Handle(
            new Presence.Contracts.Commands.UserConnectedCommand(userId, connectionId),
            cancellationToken);
    }

    public async Task UserDisconnectedAsync(Guid userId, string connectionId, CancellationToken cancellationToken = default)
    {
        await userDisconnectedHandler.Handle(
            new Presence.Contracts.Commands.UserDisconnectedCommand(userId, connectionId),
            cancellationToken);
    }

    public async Task SubscribeToPresenceAsync(Guid watcherUserId, string connectionId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        await subscribeToPresenceHandler.Handle(
            new Presence.Contracts.Commands.SubscribeToPresenceCommand(watcherUserId, connectionId, targetUserId),
            cancellationToken);
    }

    public async Task UnsubscribeFromPresenceAsync(string connectionId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        await unsubscribeFromPresenceHandler.Handle(
            new Presence.Contracts.Commands.UnsubscribeFromPresenceCommand(connectionId, targetUserId),
            cancellationToken);
    }
}
