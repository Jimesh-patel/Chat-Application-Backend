using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts;

namespace Platform.Auth.DependencyInjection;

public static class AuthDependencyInjectionExtensions
{
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        return services;
    }
}
