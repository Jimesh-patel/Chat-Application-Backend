using System.Security.Claims;
using Chat.Features.GetMessages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Commands;

namespace Chat.Features.SendMessage;

internal static class SendMessageEndpoint
{
    public static IEndpointRouteBuilder MapSendMessage(
        this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/chat/conversations/{conversationId:guid}/messages",
            async (
                [FromRoute] Guid conversationId,
                [FromBody] SendMessageCommand command,
                ClaimsPrincipal user,
                ICommandHandler<SendMessageCommand, MessageDto> handler,
                CancellationToken cancellationToken) =>
            {
                var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdStr, out var userId))
                {
                    return Results.Unauthorized();
                }

                var finalCommand = command with
                {
                    ConversationId = conversationId,
                    SenderId = userId
                };

                var result = await handler.Handle(finalCommand, cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(new SendMessageResponse(result.Value!));
            })
            .WithTags("Chat")
            .WithName("SendMessage")
            .RequireAuthorization();

        return app;
    }
}
