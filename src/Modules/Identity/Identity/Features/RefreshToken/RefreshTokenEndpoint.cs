using Identity.Features.LoginUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Commands;

namespace Identity.Features.RefreshToken;

internal static class RefreshTokenEndpoint
{
    public static IEndpointRouteBuilder MapRefreshToken(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/refresh", async (
            RefreshTokenCommand command, 
            ICommandHandler<RefreshTokenCommand, LoginUserResponse> handler, 
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(command, cancellationToken);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        })
        .WithTags("Identity")
        .AllowAnonymous();

        return app;
    }
}
