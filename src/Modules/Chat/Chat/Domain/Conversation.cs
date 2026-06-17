using Chat.Domain.Events;
using Platform.Common.Entities;

namespace Chat.Domain;


public sealed class Conversation : AggregateRoot<ConversationId>
{
    private readonly Dictionary<MessageId, MessageStatus> _messageStatuses = [];

    public Conversation() { }

    public Guid ParticipantA { get; private set; }
    public Guid ParticipantB { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public bool UserATyping { get; private set; }
    public bool UserBTyping { get; private set; }

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

    public void SetUserTyping(Guid userId, bool isTyping)
    {
        if (userId == ParticipantA)
            UserATyping = isTyping;
        else if (userId == ParticipantB)
            UserBTyping = isTyping;
    }

    public void SendMessage(
        MessageId messageId,
        Guid senderId,
        Guid recipientId,
        string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        _messageStatuses[messageId] = MessageStatus.Sent;

        Raise(new MessageSent(
            Id,
            messageId,
            senderId,
            recipientId,
            content,
            DateTime.UtcNow));
    }

    public void MarkMessageSeen(MessageId messageId)
    {
        if (!_messageStatuses.TryGetValue(messageId, out var status))
        {
            throw new InvalidOperationException("Message not found.");
        }

        if (status != MessageStatus.Sent)
        {
            throw new InvalidOperationException($"Cannot transition status to Seen from {status}.");
        }

        Raise(new MessageSeen(messageId, DateTime.UtcNow));
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
        _messageStatuses[e.MessageId] = MessageStatus.Sent;
    }

    public void Apply(MessageSeen e)
    {
        _messageStatuses[e.MessageId] = MessageStatus.Seen;
    }
}
