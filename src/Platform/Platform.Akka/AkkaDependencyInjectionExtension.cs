using Akka.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Akka;

public static class AkkaDependencyInjectionExtension
{
    public static IServiceCollection AddAkkaPlatform(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AkkaOptions>(
            configuration.GetSection(AkkaOptions.SectionName));

        var akkaOptions = configuration
            .GetSection(AkkaOptions.SectionName)
            .Get<AkkaOptions>() ?? new AkkaOptions();

        services.AddAkka(akkaOptions.ActorSystemName, (builder, sp) =>
        {
            builder.AddStartup(async (system, registry) =>
            {
                var startups = sp.GetServices<IActorStartup>();

                foreach (var startup in startups)
                {
                    await startup.StartAsync(system, registry);
                }
            });
        });

        return services;
    }
}
