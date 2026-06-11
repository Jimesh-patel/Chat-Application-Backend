using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Commands;

namespace Identity.Features.RegisterUser;

internal static class RegisterUserEndpoint
{
    public static IEndpointRouteBuilder MapRegisterUser(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/identity/register",
            async (
                RegisterUserCommand command,
                ICommandHandler<RegisterUserCommand, Guid> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(
                    new RegisterUserResponse(
                        result.Value!));
            });

        return app;
    }
}