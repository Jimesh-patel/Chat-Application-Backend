using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Hosting;
using Platform.Akka;
using Presence.Features.Broadcasting;

namespace Presence.Infrastructure.Actors;

public sealed class PresenceActorStartup : IActorStartup
{
    public Task StartAsync(ActorSystem system, IActorRegistry registry)
    {
        var resolver = DependencyResolver.For(system);

        var presenceManager = system.ActorOf(
            resolver.Props<UserPresenceManagerActor>(),
            "user-presence-manager");

        registry.Register<UserPresenceManagerActor>(presenceManager);

        var eventBroadcaster = system.ActorOf(
            resolver.Props<PresenceEventBroadcasterActor>(),
            "presence-event-broadcaster");

        registry.Register<PresenceEventBroadcasterActor>(eventBroadcaster);

        return Task.CompletedTask;
    }
}
