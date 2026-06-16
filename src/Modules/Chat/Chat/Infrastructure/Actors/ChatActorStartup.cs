using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Hosting;
using Chat.Infrastructure.Actors;
using Platform.Akka;

namespace Chat.Infrastructure;

public sealed class ChatActorStartup : IActorStartup
{
    public Task StartAsync(
        ActorSystem system,
        IActorRegistry registry)
    {
        var resolver =
            DependencyResolver.For(system);

        var conversationManager =
            system.ActorOf(
                resolver.Props<ConversationManagerActor>(),
                "conversation-manager");

        registry.Register<ConversationManagerActor>(
            conversationManager);

        return Task.CompletedTask;
    }
}