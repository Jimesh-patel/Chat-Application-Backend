using Identity.Domain;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Projections;

internal static class ProjectionDependencyInjectionExtensions
{
    public static IServiceCollection AddUserProjection(
        this IServiceCollection services)
    {
        services.ConfigureMarten(options =>
        {
            options.Projections.Snapshot<User>(
                SnapshotLifecycle.Inline);

            options.Projections.Add<UserProjection>(
                ProjectionLifecycle.Inline);
        });

        return services;
    }
}
