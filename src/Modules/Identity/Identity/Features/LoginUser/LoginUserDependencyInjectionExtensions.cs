using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Commands;

namespace Identity.Features.LoginUser;

internal static class LoginUserDependencyInjectionExtensions
{
    public static IServiceCollection AddLoginUser(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<LoginUserCommand, LoginUserResult>, LoginUserHandler>();
        return services;
    }
}
