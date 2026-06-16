using Akka.Actor;
using Akka.DependencyInjection;
using Serilog;

namespace Chat.Infrastructure.Actors;

public sealed class ConversationManagerActor : ReceiveActor
{
    public ConversationManagerActor()
    {
        Receive<GetConversationActor>(HandleGetConversationActor);
    }

    private void HandleGetConversationActor(GetConversationActor message)
    {
        var actorName =
            $"conversation-{message.ConversationId}";

        var conversationActor =
            Context.Child(actorName);

        if (conversationActor.IsNobody())
        {
            var resolver =
                DependencyResolver.For(Context.System);

            var props =
                resolver.Props<ConversationActor>(
                    new Chat.Domain.ConversationId(message.ConversationId));

            conversationActor =
                Context.ActorOf(
                    props,
                    actorName);

            Log.Information(
                "Created ConversationActor {Path}",
                conversationActor.Path);
        }

        Sender.Tell(conversationActor);
    }

    protected override void PreStart()
    {
        Log.Information(
            "ConversationManagerActor [{Path}] started.",
            Self.Path);

        base.PreStart();
    }

    protected override void PostStop()
    {
        Log.Information(
            "ConversationManagerActor [{Path}] stopped.",
            Self.Path);

        base.PostStop();
    }
}