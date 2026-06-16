using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Platform.Contracts.Queries;

namespace Chat.Features.FindAllConversations;

/// <summary>
/// Minimal API endpoint for querying all conversations for the logged in user.
/// <para>
/// GET /api/chat/conversations
/// </para>
/// </summary>
internal static class FindAllConversationsEndpoint
{
    public static IEndpointRouteBuilder MapFindAllConversations(
        this IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/chat/conversations",
            async (
                ClaimsPrincipal user,
                IQueryHandler<FindAllConversationsQuery, IReadOnlyList<ConversationResponse>> handler,
                CancellationToken cancellationToken) =>
            {
                var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdStr, out var userId))
                {
                    return Results.Unauthorized();
                }

                var query = new FindAllConversationsQuery(userId);
                var result = await handler.Handle(query, cancellationToken);

                if (result.IsFailure)
                {
                    return Results.BadRequest(result.Error);
                }

                return Results.Ok(result.Value!);
            })
            .WithTags("Chat")
            .WithName("FindAllConversations")
            .RequireAuthorization();

        return app;
    }
}
