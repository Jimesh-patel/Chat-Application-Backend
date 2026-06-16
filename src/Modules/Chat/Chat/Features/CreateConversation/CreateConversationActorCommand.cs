using Chat.Domain;

namespace Chat.Features.CreateConversation;

/// <summary>
/// Internal message sent to <see cref="Chat.Infrastructure.Actors.ConversationActor"/>
/// via Akka Ask pattern to initialize a conversation.
/// </summary>
internal sealed record CreateConversationActorCommand(
    ConversationId ConversationId,
    Guid ParticipantA,
    Guid ParticipantB);
