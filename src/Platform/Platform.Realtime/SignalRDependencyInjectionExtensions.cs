using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Platform.Realtime.Abstractions;
using Platform.Realtime.Hubs;
using Platform.Realtime.Providers;
using Platform.Realtime.Services;

namespace Platform.Realtime;

public static class SignalRDependencyInjectionExtensions
{
    public static IServiceCollection AddPlatformSignalR(
        this IServiceCollection services)
    {
        services.AddSignalR();
        
        services.AddSingleton<IUserIdProvider, JwtUserIdProvider>();
        services.AddTransient<IRealtimeNotifier, SignalRRealtimeNotifier>();

        return services;
    }

    public static IEndpointRouteBuilder MapPlatformSignalR(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<ChatHub>("/hubs/chat");

        return endpoints;
    }
}
