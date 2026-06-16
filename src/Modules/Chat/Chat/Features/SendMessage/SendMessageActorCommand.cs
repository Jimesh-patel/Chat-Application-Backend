using Chat.Domain;

namespace Chat.Features.SendMessage;

/// <summary>
/// Internal message sent to <see cref="Chat.Infrastructure.Actors.ConversationActor"/>
/// via Akka Ask pattern to append a message.
/// </summary>
internal sealed record SendMessageActorCommand(
    ConversationId ConversationId,
    MessageId MessageId,
    Guid SenderId,
    Guid RecipientId,
    string Content);
