using Identity.Features.LoginUser;
using Identity.Features.LogoutUser;
using Identity.Features.RefreshToken;
using Identity.Features.RegisterUser;
using Identity.Infractusture;
using Identity.Projections;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Modules;

namespace Identity;

public sealed class IdentityModule : IModule
{
    public IServiceCollection RegisterModule(
    IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddRegisterUser();
        services.AddLoginUser();
        services.AddLogoutUser();
        services.AddRefreshToken();
        services.AddIdentityInfractrusture();
        services.AddUserProjection();
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(
        IEndpointRouteBuilder endpoints)
    {
        endpoints.MapRegisterUser();
        endpoints.MapLoginUser();
        endpoints.MapLogoutUser();
        endpoints.MapRefreshToken();

        return endpoints;
    }

}