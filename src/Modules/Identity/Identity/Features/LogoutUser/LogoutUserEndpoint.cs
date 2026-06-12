using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Platform.Common.Results;
using Platform.Contracts.Commands;
using System.Security.Claims;

namespace Identity.Features.LogoutUser;

internal static class LogoutUserEndpoint
{
    public static IEndpointRouteBuilder MapLogoutUser(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/logout", async (
            ClaimsPrincipal user,
            ICommandHandler<LogoutUserCommand> handler,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Results.BadRequest(new Error("Identity.NoRefreshToken", "No refresh token found in cookies."));
            }

            var command = new LogoutUserCommand(userId, refreshToken);
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            httpContext.Response.Cookies.Delete("refreshToken");

            return Results.Ok();
        })
        .WithTags("Identity")
        .RequireAuthorization();

        return app;
    }
}
