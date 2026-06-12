using Identity.Features.LoginUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Commands;
using System.Security.Claims;

namespace Identity.Features.RefreshToken;

internal static class RefreshTokenEndpoint
{
    public static IEndpointRouteBuilder MapRefreshToken(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/refresh", async (
            ClaimsPrincipal user,
            ICommandHandler<RefreshTokenCommand, LoginUserResult> handler, 
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            if (!httpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Results.Unauthorized();
            }

            var command = new RefreshTokenCommand(userId, refreshToken);
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            var loginResult = result.Value!;

            httpContext.Response.Cookies.Append("refreshToken", loginResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Results.Ok(new LoginUserResponse(loginResult.AccessToken));
        })
        .WithTags("Identity")
        .RequireAuthorization();

        return app;
    }
}
