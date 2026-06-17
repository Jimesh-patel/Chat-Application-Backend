using Chat.Domain;
using Chat.Domain.Events;
using Chat.ReadModels;
using Marten.Events.Projections;

namespace Chat.Projections;

/// <summary>
/// Projects <see cref="MessageSent"/> events into <see cref="MessageReadModel"/> documents.
/// </summary>
public sealed partial class MessageProjection : MultiStreamProjection<MessageReadModel, Guid>
{
    public MessageProjection()
    {
        Identity<MessageSent>(e => e.MessageId.Value);
        Identity<MessageSeen>(e => e.MessageId.Value);
    }

    public MessageReadModel Create(MessageSent @event)
    {
        return new MessageReadModel
        {
            Id = @event.MessageId.Value,
            ConversationId = @event.ConversationId.Value,
            SenderId = @event.SenderId,
            RecipientId = @event.RecipientId,
            Content = @event.Content,
            Status = MessageStatus.Sent,
            SentAtUtc = @event.SentAtUtc
        };
    }

    public void Apply(MessageSeen @event, MessageReadModel model)
    {
        model.Status = MessageStatus.Seen;
        model.SeenAtUtc = @event.SeenAtUtc;
    }
}
