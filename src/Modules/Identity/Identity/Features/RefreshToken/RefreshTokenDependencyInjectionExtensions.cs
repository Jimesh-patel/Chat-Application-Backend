using Microsoft.Extensions.DependencyInjection;
using Identity.Features.LoginUser;
using Platform.Contracts.Commands;

namespace Identity.Features.RefreshToken;

internal static class RefreshTokenDependencyInjectionExtensions
{
    public static IServiceCollection AddRefreshToken(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RefreshTokenCommand, LoginUserResult>, RefreshTokenHandler>();
        return services;
    }
}
