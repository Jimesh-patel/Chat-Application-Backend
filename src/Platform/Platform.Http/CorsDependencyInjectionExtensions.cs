using Microsoft.Extensions.DependencyInjection;

namespace Platform.Http;

public static class CorsDependencyInjectionExtensions
{
    private const string CorsPolicyName = "Frontend";

    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static string PolicyName => CorsPolicyName;
}