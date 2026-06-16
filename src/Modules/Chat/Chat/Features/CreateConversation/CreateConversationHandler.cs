using Akka.Actor;
using Akka.Hosting;
using Chat.Domain;
using Chat.Infrastructure.Actors;
using Chat.ReadModels;
using Identity.Contracts.APIs;
using Marten;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using Serilog;

namespace Chat.Features.CreateConversation;

/// <summary>
/// Handles <see cref="CreateConversationCommand"/> by checking if a conversation already exists,
/// and if not, routing through the actor system. Returns full details of ParticipantB.
/// </summary>
internal sealed class CreateConversationHandler(
    IActorRegistry actorRegistry,
    IQuerySession querySession,
    IIdentityApi identityApi)
    : ICommandHandler<CreateConversationCommand, CreateConversationResponse>
{
    private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(10);

    public async Task<Result<CreateConversationResponse>> Handle(
        CreateConversationCommand command,
        CancellationToken cancellationToken)
    {
        Log.Information(
            "Handling CreateConversationCommand for Participants {ParticipantA} and {ParticipantB}",
            command.ParticipantA,
            command.ParticipantB);

        try
        {
            var user = await identityApi.GetUserAsync(command.ParticipantB, cancellationToken);
            var participantB = user.Value;
            if (participantB is null)
            {
                return Result<CreateConversationResponse>.Failure(
                    new Error("Chat.ParticipantNotFound", "Participant B was not found."));
            }

            var existingConversation = await querySession.Query<ConversationReadModel>()
                .FirstOrDefaultAsync(c =>
                    (c.ParticipantA == command.ParticipantA && c.ParticipantB == command.ParticipantB) ||
                    (c.ParticipantA == command.ParticipantB && c.ParticipantB == command.ParticipantA),
                    cancellationToken);

            if (existingConversation is null)
            {
                var conversationId = ConversationId.New();

                var manager = actorRegistry.Get<ConversationManagerActor>();

                var conversationActor = await manager.Ask<IActorRef>(
                    new GetConversationActor(conversationId.Value),
                    AskTimeout,
                    cancellationToken);

                var actorCommand = new CreateConversationActorCommand(
                    conversationId,
                    command.ParticipantA,
                    command.ParticipantB);

                var result = await conversationActor.Ask<Result<Guid>>(
                    actorCommand, 
                    AskTimeout, 
                    cancellationToken: cancellationToken);

                if (result.IsFailure)
                {
                    Log.Warning(
                        "ConversationActor returned failure for new conversation: {Error}",
                        result.Error);
                    return Result<CreateConversationResponse>.Failure(result.Error!);
                }

                return Result<CreateConversationResponse>.Success(
                    new CreateConversationResponse(result.Value!, participantB));
            }

            return Result<CreateConversationResponse>.Success(
                new CreateConversationResponse(existingConversation.Id, participantB));
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "Unexpected error creating conversation. Message: {Message}, Inner: {Inner}",
                ex.Message,
                ex.InnerException?.Message);

            return Result<CreateConversationResponse>.Failure(
                new Error(
                    "Chat.CreateConversationFailed",
                    "An unexpected error occurred while creating the conversation."));
        }
    }
}
