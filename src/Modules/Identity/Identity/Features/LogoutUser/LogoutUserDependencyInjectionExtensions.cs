using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;

namespace Identity.Features.LogoutUser;

internal static class LogoutUserDependencyInjectionExtensions
{
    public static IServiceCollection AddLogoutUser(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<LogoutUserCommand>, LogoutUserHandler>();
        return services;
    }
}
