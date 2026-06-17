using Akka.Actor;
using Chat.Domain;
using Chat.Features.CreateConversation;
using Chat.Features.MarkSeen;
using Chat.Features.SendMessage;
using Chat.Features.UserTyping;
using Marten;
using Platform.Akka.Actors;
using Platform.Common.Results;
using Serilog;

namespace Chat.Infrastructure.Actors;

public sealed class ConversationActor : ChatActorBase
{
    private readonly ConversationId _conversationId;
    private readonly IDocumentStore _store;

    private Conversation? _conversation;

    public ConversationActor(
        ConversationId conversationId,
        IDocumentStore store)
    {
        _conversationId = conversationId;
        _store = store;

        ReceiveAsync<CreateConversationActorCommand>(
            HandleCreateConversation);

        ReceiveAsync<SendMessageActorCommand>(
            HandleSendMessage);

        ReceiveAsync<MarkMessageSeenActorCommand>(
            HandleMarkSeen);

        Receive<SetUserTypingActorCommand>(
            HandleSetUserTyping);
    }

    protected override void OnPreStart()
    {
        LoadConversation();

        LogInfo(
            "Loaded conversation {ConversationId}",
            _conversationId);
    }

    private void LoadConversation()
    {
        using var session =
            _store.QuerySession();

        _conversation =
            session.Events
                .AggregateStreamAsync<Conversation>(
                    _conversationId.Value)
                .GetAwaiter()
                .GetResult();
    }

    private async Task HandleCreateConversation(
        CreateConversationActorCommand cmd)
    {
        try
        {
            if (_conversation is not null)
            {
                Sender.Tell(
                    Result<Guid>.Failure(
                        new Error(
                            "Chat.ConversationExists",
                            "Conversation already exists.")));

                return;
            }

            _conversation =
                Conversation.Start(
                    _conversationId,
                    cmd.ParticipantA,
                    cmd.ParticipantB);

            await using var session =
                _store.LightweightSession();

            session.Events.StartStream<Conversation>(
                _conversationId.Value,
                _conversation.DomainEvents);

            await session.SaveChangesAsync();

            Sender.Tell(
                Result<Guid>.Success(
                    _conversationId.Value));
        }
        catch (Exception ex)
        {
            LogError(
                ex,
                "Failed creating conversation {ConversationId}",
                _conversationId);

            Sender.Tell(
                Result<Guid>.Failure(
                    new Error(
                        "Chat.CreateFailed",
                        "Failed to create conversation.")));
        }
    }

    private async Task HandleSendMessage(
        SendMessageActorCommand cmd)
    {
        try
        {
            if (_conversation is null)
            {
                Sender.Tell(
                    Result<Guid>.Failure(
                        new Error(
                            "Chat.ConversationNotFound",
                            "Conversation not found.")));

                return;
            }

            _conversation.SendMessage(
                cmd.MessageId,
                cmd.SenderId,
                cmd.RecipientId,
                cmd.Content);

            await using var session = _store.LightweightSession();

            session.Events.Append(
                _conversationId.Value,
                _conversation.DomainEvents);

            await session.SaveChangesAsync();

            Sender.Tell(
                Result<Guid>.Success(
                    cmd.MessageId.Value));
        }
        catch (Exception ex)
        {
            LogError(
                ex,
                "Failed saving message {MessageId}",
                cmd.MessageId);

            Sender.Tell(
                Result<Guid>.Failure(
                    new Error(
                        "Chat.PersistenceFailed",
                        "Failed to persist message.")));
        }
    }

    private async Task HandleMarkSeen(
        MarkMessageSeenActorCommand cmd)
    {
        try
        {
            if (_conversation is null)
            {
                Sender.Tell(Result<Guid>.Failure(new Error("Chat.ConversationNotFound", "Conversation not found.")));
                return;
            }

            _conversation.MarkMessageSeen(cmd.MessageId);
            Log.Information("[Mark Seen]: Mark msg seen - {id}", cmd.MessageId);

            await using var session = _store.LightweightSession();
            session.Events.Append(_conversationId.Value, _conversation.DomainEvents);
            await session.SaveChangesAsync();

            Sender.Tell(Result<Guid>.Success(cmd.MessageId.Value));
        }
        catch (InvalidOperationException ex)
        {
            Sender.Tell(Result<Guid>.Failure(new Error("Chat.InvalidStatusTransition", ex.Message)));
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed marking message {MessageId} as seen", cmd.MessageId);
            Sender.Tell(Result<Guid>.Failure(new Error("Chat.PersistenceFailed", "Failed to persist status.")));
        }
    }

    private void HandleSetUserTyping(SetUserTypingActorCommand cmd)
    {
        try
        {
            if (_conversation is null)
            {
                Sender.Tell(Result<Guid>.Failure(new Error("Chat.ConversationNotFound", "Conversation not found.")));
                return;
            }

            _conversation.SetUserTyping(cmd.UserId, cmd.IsTyping);
            var oppositeId = _conversation.ParticipantA == cmd.UserId ? _conversation.ParticipantB : _conversation.ParticipantA;

            Sender.Tell(Result<Guid>.Success(oppositeId));
        }
        catch (Exception ex)
        {
            LogError(ex, "Failed setting typing status for user {UserId}", cmd.UserId);
            Sender.Tell(Result<Guid>.Failure(new Error("Chat.SetTypingFailed", "Failed to set typing status.")));
        }
    }

    protected override void OnPostStop()
    {
        LogInfo(
            "Conversation actor stopped for {ConversationId}",
            _conversationId);
    }
}