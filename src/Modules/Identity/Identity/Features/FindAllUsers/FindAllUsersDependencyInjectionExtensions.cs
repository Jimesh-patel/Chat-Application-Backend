using Microsoft.Extensions.DependencyInjection;
using Platform.Contracts.Queries;

namespace Identity.Features.FindAllUsers;

internal static class FindAllUsersDependencyInjectionExtensions
{
    public static IServiceCollection AddFindAllUsers(
        this IServiceCollection services)
    {
        services.AddScoped<
            IQueryHandler<FindAllUsersQuery, IReadOnlyList<UserResponse>>,
            FindAllUsersHandler>();

        return services;
    }
}
