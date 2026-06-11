using Identity.Contracts.APIs;
using Identity.Features.RegisterUser;
using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;

namespace Identity.Infractusture;

internal static class IdentityInfrastructureDependencyInjectionExtensions
{
    public static IServiceCollection AddIdentityInfractrusture(
        this IServiceCollection services)
    {
        services.AddScoped<IIdentityApi, IdentityApi>();

        return services;
    }
}
