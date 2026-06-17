using Akka.Actor;
using Akka.Hosting;
using Chat.Domain;
using Chat.Infrastructure.Actors;
using Chat.ReadModels;
using Marten;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using Platform.Realtime.Abstractions;
using Serilog;

namespace Chat.Features.MarkSeen;

internal sealed class MarkMessageSeenHandler(
    IActorRegistry actorRegistry,
    IQuerySession querySession,
    IRealtimeNotifier realtimeNotifier)
    : ICommandHandler<MarkMessageSeenCommand, Guid>
{
    private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(10);

    public async Task<Result<Guid>> Handle(
        MarkMessageSeenCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var manager = actorRegistry.Get<ConversationManagerActor>();

            var conversationActor = await manager.Ask<IActorRef>(
                new GetConversationActor(command.ConversationId),
                AskTimeout,
                cancellationToken);

            var result = await conversationActor.Ask<Result<Guid>>(
                new MarkMessageSeenActorCommand(
                    new ConversationId(command.ConversationId),
                    new MessageId(command.MessageId)),
                AskTimeout,
                cancellationToken);

            if (result.IsFailure)
            {
                return result;
            }

            var senderId = await querySession.Query<MessageReadModel>()
                .Where(m => m.Id == command.MessageId)
                .Select(m => m.SenderId)
                .FirstOrDefaultAsync(cancellationToken);

            if (senderId != Guid.Empty)
            {
               await realtimeNotifier.SendToUserAsync(
                    senderId,
                    "MessageSeen",
                    new { command.ConversationId, command.MessageId, SeenAtUtc = DateTime.UtcNow },
                    cancellationToken);
            }

            return Result<Guid>.Success(result.Value);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error marking message seen");
            return Result<Guid>.Failure(
                new Error("Chat.MarkSeenFailed", "An unexpected error occurred."));
        }
    }
}
