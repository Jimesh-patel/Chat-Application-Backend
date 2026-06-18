using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;
using Platform.Contracts.Modules;
using Presence.Contracts.Commands;
using Presence.Features.SubscribeToPresence;
using Presence.Features.UnsubscribeFromPresence;
using Presence.Features.UserConnected;
using Presence.Features.UserDisconnected;
using Presence.Infrastructure;
using Presence.Infrastructure.Actors;
using Presence.Projections;

namespace Presence;

public sealed class PresenceModule : IModule
{
    public IServiceCollection RegisterModule(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IPresenceSubscriptionManager, PresenceSubscriptionManager>();
        
        // Register Command Handlers
        services.AddTransient<ICommandHandler<UserConnectedCommand, Guid>, UserConnectedHandler>();
        services.AddTransient<ICommandHandler<UserDisconnectedCommand, Guid>, UserDisconnectedHandler>();
        services.AddTransient<ICommandHandler<SubscribeToPresenceCommand, Guid>, SubscribeToPresenceHandler>();
        services.AddTransient<ICommandHandler<UnsubscribeFromPresenceCommand, Guid>, UnsubscribeFromPresenceHandler>();

        services.AddSingleton<Platform.Akka.IActorStartup, PresenceActorStartup>();

        services.AddPresenceProjection();

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // No HTTP endpoints for Presence module in V1
        return endpoints;
    }
}
