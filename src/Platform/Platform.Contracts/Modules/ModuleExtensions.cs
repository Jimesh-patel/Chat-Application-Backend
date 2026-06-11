using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Platform.Contracts.Modules;

public static class ModuleExtensions
{
    private static readonly List<IModule> Modules = [];

    public static IServiceCollection AddModules(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        var modules = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                typeof(IModule).IsAssignableFrom(t) &&
                t is { IsInterface: false, IsAbstract: false })
            .Select(t => (IModule)Activator.CreateInstance(t)!)
            .ToList();

        Modules.AddRange(modules);

        foreach (var module in modules)
        {
            module.RegisterModule(
                services,
                configuration);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapModules(
        this IEndpointRouteBuilder app)
    {
        foreach (var module in Modules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }
}