using Chat.Features.GetMessages;
using Chat.Features.MarkSeen;
using Chat.Features.SendMessage;
using Platform.Contracts.Commands;
using Platform.Realtime.Abstractions;

namespace Chat.Infrastructure.Realtime;

internal sealed class ChatHubDispatcher(
    ICommandHandler<SendMessageCommand, MessageDto> sendMessageHandler,
    ICommandHandler<MarkMessageSeenCommand, Guid> markSeenHandler)
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
}
