using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Queries;

namespace Identity.Features.FindAllUsers;

/// <summary>
/// Minimal API endpoint for querying all users.
/// <para>
/// GET /api/identity/users
/// </para>
/// </summary>
internal static class FindAllUsersEndpoint
{
    public static IEndpointRouteBuilder MapFindAllUsers(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/identity/users",
            async (
                IQueryHandler<FindAllUsersQuery, IReadOnlyList<UserResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new FindAllUsersQuery();
                var result = await handler.Handle(query, cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value!);
            })
            .WithTags("Identity")
            .WithName("FindAllUsers")
            .RequireAuthorization();

        return app;
    }
}
