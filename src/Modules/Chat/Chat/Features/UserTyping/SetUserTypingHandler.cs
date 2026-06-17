using Akka.Actor;
using Akka.Hosting;
using Chat.Infrastructure.Actors;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using Platform.Realtime.Abstractions;
using Serilog;

namespace Chat.Features.UserTyping;

internal sealed class SetUserTypingHandler(
    IActorRegistry actorRegistry,
    IRealtimeNotifier realtimeNotifier)
    : ICommandHandler<SetUserTypingCommand, Guid>
{
    private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(10);

    public async Task<Result<Guid>> Handle(
        SetUserTypingCommand command,
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
                new SetUserTypingActorCommand(
                    command.ConversationId,
                    command.UserId,
                    command.IsTyping),
                AskTimeout,
                cancellationToken);

            if (result.IsFailure)
            {
                return result;
            }

            var oppositeUserId = result.Value;
            var eventName = command.IsTyping ? "UserTyping" : "StopTyping";

            await realtimeNotifier.SendToUserAsync(
                oppositeUserId,
                eventName,
                new { command.ConversationId, command.UserId },
                cancellationToken);

            return Result<Guid>.Success(oppositeUserId);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error setting user typing status");
            return Result<Guid>.Failure(
                new Error("Chat.SetTypingFailed", "An unexpected error occurred."));
        }
    }
}
