using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Presence.ReadModels;

namespace Presence.Projections;

internal static class PresenceProjectionDependencyInjectionExtensions
{
    public static IServiceCollection AddPresenceProjection(
        this IServiceCollection services)
    {
        services.ConfigureMarten(options =>
        {
            options.Projections.Add<PresenceProjection>(ProjectionLifecycle.Inline);
        });

        return services;
    }
}
