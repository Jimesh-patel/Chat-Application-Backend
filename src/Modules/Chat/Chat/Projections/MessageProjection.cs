using Chat.Domain.Events;
using Chat.ReadModels;
using Marten.Events.Projections;

namespace Chat.Projections;

/// <summary>
/// Projects <see cref="MessageSent"/> events into <see cref="MessageReadModel"/> documents.
/// </summary>
public sealed partial class MessageProjection : EventProjection
{
    public MessageReadModel Create(MessageSent @event)
    {
        return new MessageReadModel
        {
            Id = @event.MessageId.Value,
            ConversationId = @event.ConversationId.Value,
            SenderId = @event.SenderId,
            RecipientId = @event.RecipientId,
            Content = @event.Content,
            SentAtUtc = @event.SentAtUtc
        };
    }
}
