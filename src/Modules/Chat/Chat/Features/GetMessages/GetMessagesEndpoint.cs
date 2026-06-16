using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Queries;

namespace Chat.Features.GetMessages;

/// <summary>
/// Minimal API endpoint to retrieve all messages in a conversation.
/// </summary>
internal static class GetMessagesEndpoint
{
    public static IEndpointRouteBuilder MapGetMessages(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/chat/conversations/{conversationId:guid}/messages",
            async (
                [FromRoute] Guid conversationId,
                IQueryHandler<GetMessagesQuery, IReadOnlyList<MessageDto>> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetMessagesQuery(conversationId);
                var result = await handler.Handle(query, cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value!);
            })
            .WithTags("Chat")
            .WithName("GetMessages")
            .WithSummary("Retrieves all messages for a specific conversation.")
            .RequireAuthorization();

        return app;
    }
}
