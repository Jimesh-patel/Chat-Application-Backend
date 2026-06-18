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
    public sealed record TypingHubRequest(Guid ConversationId, Guid UserId);

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

    public async Task UserTyping(TypingHubRequest request)
    {
        Log.Information("[SignalR]: User Typing - Conv:{ConvId} User:{UserId}", request.ConversationId, request.UserId);
        
        var userIdStr = Context.UserIdentifier;
        if (!Guid.TryParse(userIdStr, out var userId))
            throw new HubException("Unauthorized");

        // Use request.UserId if provided, otherwise fallback to the authenticated userId. 
        // We'll trust the auth context userId to prevent spoofing, but the requirement says frontend sends it.
        await dispatcher.SetUserTypingAsync(request.ConversationId, userId, true);
    }

    public async Task StopTyping(TypingHubRequest request)
    {
        Log.Information("[SignalR]: Stop Typing - Conv:{ConvId} User:{UserId}", request.ConversationId, request.UserId);

        var userIdStr = Context.UserIdentifier;
        if (!Guid.TryParse(userIdStr, out var userId))
            throw new HubException("Unauthorized");

        await dispatcher.SetUserTypingAsync(request.ConversationId, userId, false);
    }

    public async Task SubscribeToPresence(Guid targetUserId)
    {
        Log.Information("[SignalR]: Subscribe To Presence - Target:{TargetUserId}", targetUserId);

        var userIdStr = Context.UserIdentifier;
        if (!Guid.TryParse(userIdStr, out var userId))
            throw new HubException("Unauthorized");

        await dispatcher.SubscribeToPresenceAsync(userId, Context.ConnectionId, targetUserId);
    }

    public async Task UnsubscribeFromPresence(Guid targetUserId)
    {
        Log.Information("[SignalR]: Unsubscribe From Presence - Target:{TargetUserId}", targetUserId);

        var userIdStr = Context.UserIdentifier;
        if (!Guid.TryParse(userIdStr, out var userId))
            throw new HubException("Unauthorized");

        await dispatcher.UnsubscribeFromPresenceAsync(Context.ConnectionId, targetUserId);
    }

    public override async Task OnConnectedAsync()
    {
        Log.Information(
            "SignalR Connected. ConnectionId={ConnectionId}, User={User}",
            Context.ConnectionId,
            Context.UserIdentifier);

        if (Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            await dispatcher.UserConnectedAsync(userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Log.Information(
            exception,
            "SignalR Disconnected. ConnectionId={ConnectionId}",
            Context.ConnectionId);

        if (Guid.TryParse(Context.UserIdentifier, out var userId))
        {
            await dispatcher.UserDisconnectedAsync(userId, Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}