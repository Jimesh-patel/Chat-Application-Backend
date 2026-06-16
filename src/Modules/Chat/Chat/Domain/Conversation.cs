using Chat.Domain.Events;
using Platform.Common.Entities;

namespace Chat.Domain;


public sealed class Conversation : AggregateRoot<ConversationId>
{
    public Conversation() { }

    public Guid ParticipantA { get; private set; }
    public Guid ParticipantB { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime LastMessageAtUtc { get; private set; }
    public int MessageCount { get; private set; }


    public static Conversation Start(
        ConversationId id,
        Guid participantA,
        Guid participantB)
    {
        var conversation = new Conversation
        {
            Id = id,
            ParticipantA = participantA,
            ParticipantB = participantB,
            CreatedAtUtc = DateTime.UtcNow
        };

        conversation.Raise(new ConversationStarted(
            id,
            participantA,
            participantB,
            conversation.CreatedAtUtc));

        return conversation;
    }

    public void SendMessage(
        MessageId messageId,
        Guid senderId,
        Guid recipientId,
        string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        Raise(new MessageSent(
            Id,
            messageId,
            senderId,
            recipientId,
            content,
            DateTime.UtcNow));
    }

    public void Apply(ConversationStarted e)
    {
        Id = e.ConversationId;
        ParticipantA = e.ParticipantA;
        ParticipantB = e.ParticipantB;
        CreatedAtUtc = e.StartedAtUtc;
    }

    public void Apply(MessageSent e)
    {
        LastMessageAtUtc = e.SentAtUtc;
        MessageCount++;
    }
}
