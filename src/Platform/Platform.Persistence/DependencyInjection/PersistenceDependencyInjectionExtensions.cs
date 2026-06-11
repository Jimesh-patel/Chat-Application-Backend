using JasperFx;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Persistence.Options;

namespace Platform.Persistence.DependencyInjection;

public static class PersistenceDependencyInjectionExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>()
            ?? throw new InvalidOperationException(
                "Database configuration is missing.");

        services.AddMarten(cfg =>
        {
            cfg.Connection(options.ConnectionString);

            cfg.DatabaseSchemaName = "chatapp";

            cfg.Events.DatabaseSchemaName = "chatapp_events";

            cfg.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
        });

        return services;
    }
}