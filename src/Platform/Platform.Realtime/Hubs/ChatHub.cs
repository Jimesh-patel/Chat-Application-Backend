using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;

using Platform.Realtime.Abstractions;

namespace Platform.Realtime.Hubs;

[Authorize]
public sealed class ChatHub(IChatHubDispatcher dispatcher) : Hub
{
    public sealed record SendMessageHubRequest(Guid ConversationId, Guid RecipientId, string Content);

    public async Task<object?> SendMessage(SendMessageHubRequest request)
    {
        Log.Information("[SignalR]: Send Message Event : {context}", request.Content);
        var userIdStr = Context.UserIdentifier;
        if (!Guid.TryParse(userIdStr, out var userId))
        {
            throw new HubException("Unauthorized");
        }

        return await dispatcher.SendMessageAsync(
            userId,
            request.ConversationId,
            request.RecipientId,
            request.Content);
    }

    public async Task MarkMessageSeen(Guid conversationId, Guid messageId)
    {
        Log.Information("[SignalR]: Mark Message Seen - {id}", messageId);
        await dispatcher.MarkMessageSeenAsync(conversationId, messageId);
    }

    public override async Task OnConnectedAsync()
    {
        Log.Information(
            "SignalR Connected. ConnectionId={ConnectionId}, User={User}",
            Context.ConnectionId,
            Context.UserIdentifier);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Log.Information(
            exception,
            "SignalR Disconnected. ConnectionId={ConnectionId}",
            Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }
}