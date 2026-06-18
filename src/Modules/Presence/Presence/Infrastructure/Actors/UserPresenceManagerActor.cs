using Akka.Actor;
using Akka.DependencyInjection;
using Marten;
using Serilog;

namespace Presence.Infrastructure.Actors;

public sealed class UserPresenceManagerActor : ReceiveActor
{
    private readonly IDocumentStore _store;

    public UserPresenceManagerActor(IDocumentStore store)
    {
        _store = store;
        Receive<GetUserPresenceActor>(HandleGetUserPresenceActor);
    }

    private void HandleGetUserPresenceActor(GetUserPresenceActor message)
    {
        var actorName = $"user-presence-{message.UserId}";
        var presenceActor = Context.Child(actorName);

        if (presenceActor.IsNobody())
        {
            var props = Props.Create(() => new UserPresenceActor(message.UserId, _store));
            presenceActor = Context.ActorOf(props, actorName);
            Log.Information("Created UserPresenceActor {Path}", presenceActor.Path);
        }

        Sender.Tell(presenceActor);
    }

    protected override void PreStart()
    {
        Log.Information("UserPresenceManagerActor [{Path}] started.", Self.Path);
        base.PreStart();
    }
}
