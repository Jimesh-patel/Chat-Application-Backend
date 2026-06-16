using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Commands;

namespace Chat.Features.CreateConversation;

/// <summary>
/// Minimal API endpoint for creating a new conversation.
/// </summary>
internal static class CreateConversationEndpoint
{
    public static IEndpointRouteBuilder MapCreateConversation(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/chat/conversations",
            async (
                CreateConversationCommand command,
                ClaimsPrincipal user,
                ICommandHandler<CreateConversationCommand, CreateConversationResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdStr, out var userId))
                {
                    return Results.Unauthorized();
                }

                var finalCommand = command with { ParticipantA = userId };

                var result = await handler.Handle(finalCommand, cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value!);
            })
            .WithTags("Chat")
            .WithName("CreateConversation")
            .RequireAuthorization();

        return app;
    }
}
