using Chat.Domain;

namespace Chat.ReadModels;

public sealed class ConversationReadModel
{
    public ConversationId Id { get; set; }
    public Guid ParticipantA { get; set; }
    public Guid ParticipantB { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime LastMessageAtUtc { get; set; }
    public int MessageCount { get; set; }
    public List<MessageReadModel> Messages { get; set; } = [];
}
