namespace Chat.ReadModels;

/// <summary>
/// Read model representing a single message document within a conversation.
/// </summary>
public sealed class MessageReadModel
{
    public Guid Id { get; set; } // Marten uses Id as the primary key
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public Guid RecipientId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAtUtc { get; set; }
}
