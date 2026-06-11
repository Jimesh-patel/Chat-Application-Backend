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
        services.AddIdentityInfractrusture();
        services.AddUserProjection();
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(
        IEndpointRouteBuilder endpoints)
    {
        endpoints.MapRegisterUser();

        return endpoints;
    }

}