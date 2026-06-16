using Chat.Domain;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Projections;

/// <summary>
/// Registers Marten projections and snapshots for the Chat module.
/// </summary>
internal static class ConversationProjectionDependencyInjectionExtensions
{
    public static IServiceCollection AddConversationProjection(
        this IServiceCollection services)
    {
        services.ConfigureMarten(options =>
        {
            // Snapshot the Conversation aggregate so replays don't start from scratch.
            options.Projections.Snapshot<Conversation>(SnapshotLifecycle.Inline);

            // Inline projection: read model updated synchronously with each event append.
            options.Projections.Add<ConversationProjection>(ProjectionLifecycle.Inline);
            
            // Inline projection for standalone message read models.
            options.Projections.Add<MessageProjection>(ProjectionLifecycle.Inline);
        });

        return services;
    }
}
