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
            ICommandHandler<LoginUserCommand, LoginUserResponse> handler, 
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
