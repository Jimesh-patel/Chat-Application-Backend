using Chat.Domain;
using Chat.Domain.Events;
using Chat.ReadModels;
using Marten.Events.Aggregation;

namespace Chat.Projections;

/// <summary>
/// Marten single-stream projection that builds and maintains <see cref="ConversationReadModel"/>
/// from the conversation event stream.
/// </summary>
public sealed partial class ConversationProjection
    : SingleStreamProjection<ConversationReadModel, ConversationId>
{
    public static ConversationReadModel Create(ConversationStarted @event)
        => new()
        {
            Id = @event.ConversationId.Value,
            ParticipantA = @event.ParticipantA,
            ParticipantB = @event.ParticipantB,
            CreatedAtUtc = @event.StartedAtUtc,
            LastMessageAtUtc = @event.StartedAtUtc,
            MessageCount = 0,
            Messages = []
        };

    public static ConversationReadModel Apply(
        MessageSent @event,
        ConversationReadModel model)
    {
        model.LastMessageAtUtc = @event.SentAtUtc;
        model.MessageCount++;
        model.Messages.Add(new MessageReadModel
        {
            Id = @event.MessageId.Value,
            ConversationId = @event.ConversationId.Value,
            SenderId = @event.SenderId,
            RecipientId = @event.RecipientId,
            Content = @event.Content,
            SentAtUtc = @event.SentAtUtc
        });
        return model;
    }
}
