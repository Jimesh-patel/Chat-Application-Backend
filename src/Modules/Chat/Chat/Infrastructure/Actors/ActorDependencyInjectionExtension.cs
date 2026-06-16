using Microsoft.Extensions.DependencyInjection;
using Platform.Akka;

namespace Chat.Infrastructure.Actors;

internal static class ActorDependencyInjectionExtension
{
    public static IServiceCollection AddActorSystem(
        this IServiceCollection services)
    {
        services.AddSingleton<IActorStartup, ChatActorStartup>();
        return services;
    }
}
