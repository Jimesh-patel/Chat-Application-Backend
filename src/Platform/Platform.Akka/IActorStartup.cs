using Akka.Actor;
using Akka.Hosting;

namespace Platform.Akka;

public interface IActorStartup
{
    Task StartAsync(
        ActorSystem system,
        IActorRegistry registry);
}