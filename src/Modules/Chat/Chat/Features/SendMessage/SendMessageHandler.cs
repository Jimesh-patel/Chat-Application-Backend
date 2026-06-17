using Akka.Actor;
using Akka.Hosting;
using Chat.Domain;
using Chat.Features.GetMessages;
using Chat.Infrastructure.Actors;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using Platform.Realtime.Abstractions;
using Serilog;

namespace Chat.Features.SendMessage;


internal sealed class SendMessageHandler(
    IActorRegistry actorRegistry,
    IRealtimeNotifier realtimeNotifier)
    : ICommandHandler<SendMessageCommand, MessageDto>
{
    private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(10);

    public async Task<Result<MessageDto>> Handle(
    SendMessageCommand command,
    CancellationToken cancellationToken)
    {
        try
        {
            var messageId = MessageId.New();

            var manager = actorRegistry.Get<ConversationManagerActor>();

            var conversationActor = await manager.Ask<IActorRef>(
                new GetConversationActor(command.ConversationId),
                AskTimeout,
                cancellationToken);

            var result = await conversationActor.Ask<Result<Guid>>(
                new SendMessageActorCommand(
                    new ConversationId(command.ConversationId),
                    messageId,
                    command.SenderId,
                    command.RecipientId,
                    command.Content),
                AskTimeout,
                cancellationToken);

            if (result.IsFailure)
            {
                return Result<MessageDto>.Failure(result.Error!);
            }

            var messageDto = new MessageDto(
                messageId.Value,
                command.ConversationId,
                command.SenderId,
                command.Content,
                MessageStatus.Sent,
                DateTime.UtcNow,
                null);

            // Receiver realtime update
            Log.Information("MessageReceived: {context}", command.Content);
            await realtimeNotifier.SendToUserAsync(
                command.RecipientId,
                "MessageReceived",
                messageDto,
                cancellationToken);

            return Result<MessageDto>.Success(messageDto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error sending message");

            return Result<MessageDto>.Failure(
                new Error(
                    "Chat.SendMessageFailed",
                    "An unexpected error occurred while sending the message."));
        }
    }
}
