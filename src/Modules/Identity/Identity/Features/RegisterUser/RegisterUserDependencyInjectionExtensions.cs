using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;

namespace Identity.Features.RegisterUser;

internal static class RegisterUserDependencyInjectionExtensions
{
    public static IServiceCollection AddRegisterUser(
        this IServiceCollection services)
    {
        services.AddScoped<
            ICommandHandler<RegisterUserCommand, Guid>,
            RegisterUserHandler>();

        return services;
    }
}