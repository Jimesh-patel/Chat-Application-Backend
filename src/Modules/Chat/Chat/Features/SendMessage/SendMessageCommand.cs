using System.Text.Json.Serialization;
using Chat.Features.GetMessages;
using Platform.Contracts.Commands;

namespace Chat.Features.SendMessage;

/// <summary>
/// Command to send a message in a conversation.
/// </summary>
public sealed record SendMessageCommand(
    Guid RecipientId,
    string Content) : ICommand<MessageDto>
{
    // Populated by the endpoint
    [JsonIgnore]
    public Guid ConversationId { get; init; }
    [JsonIgnore]
    public Guid SenderId { get; init; }
}
