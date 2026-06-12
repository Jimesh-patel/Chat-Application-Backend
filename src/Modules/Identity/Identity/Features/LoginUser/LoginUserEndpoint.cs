using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Commands;

namespace Identity.Features.LoginUser;

internal static class LoginUserEndpoint
{
    public static IEndpointRouteBuilder MapLoginUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/login", async (
            LoginUserCommand command, 
            ICommandHandler<LoginUserCommand, LoginUserResult> handler, 
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            var loginResult = result.Value!;

            httpContext.Response.Cookies.Append("refreshToken", loginResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure HTTPS is used
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7) // Should match JwtOptions ideally, but fine for now
            });

            return Results.Ok(new LoginUserResponse(loginResult.AccessToken));
        })
        .WithTags("Identity")
        .AllowAnonymous();

        return app;
    }
}
