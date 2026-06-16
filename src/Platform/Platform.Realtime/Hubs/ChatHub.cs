using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Platform.Realtime.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        Log.Information(
            "SignalR Connected. ConnectionId={ConnectionId}, User={User}",
            Context.ConnectionId,
            Context.UserIdentifier);

        foreach (var claim in Context.User?.Claims ?? [])
        {
            Log.Information(
                "Claim {Type}={Value}",
                claim.Type,
                claim.Value);
        }

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