using System.Text.Json.Serialization;
using Platform.Contracts.Commands;

namespace Chat.Features.CreateConversation;


public sealed record CreateConversationCommand(
    Guid ParticipantB) : ICommand<CreateConversationResponse>
{
    [JsonIgnore]
    public Guid ParticipantA { get; init; }
}
