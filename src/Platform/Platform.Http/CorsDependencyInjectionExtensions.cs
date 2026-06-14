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
                    .WithOrigins("*")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    public static string PolicyName => CorsPolicyName;
}