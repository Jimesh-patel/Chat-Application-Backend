using Microsoft.AspNetCore.SignalR;
using Platform.Realtime.Abstractions;
using Platform.Realtime.Hubs;

namespace Platform.Realtime.Services;

public sealed class SignalRRealtimeNotifier(IHubContext<ChatHub> hubContext) : IRealtimeNotifier
{
    public Task SendToUserAsync(
        Guid userId,
        string eventName,
        object payload,
        CancellationToken cancellationToken = default)
    {
        return hubContext.Clients.User(userId.ToString()).SendAsync(
            eventName,
            payload,
            cancellationToken);
    }
}
