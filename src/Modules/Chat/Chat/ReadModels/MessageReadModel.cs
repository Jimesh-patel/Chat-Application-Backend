using Chat.Domain;

namespace Chat.ReadModels;

public sealed class MessageReadModel
{
    public Guid Id { get; set; } 
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageStatus Status { get; set; }
    public DateTime SentAtUtc { get; set; }
    public DateTime? SeenAtUtc { get; set; }
}
