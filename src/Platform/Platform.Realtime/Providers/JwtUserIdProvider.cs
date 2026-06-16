using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Platform.Realtime.Providers;

public sealed class JwtUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
