using Akka.Actor;
using Marten;
using Presence.Contracts.Events;
using Presence.Domain;

namespace Presence.Infrastructure.Actors;

public sealed class UserPresenceActor : ReceiveActor
{
    private readonly Guid _userId;
    private readonly IDocumentStore _store;
    private readonly PresenceState _state;

    public UserPresenceActor(Guid userId, IDocumentStore store)
    {
        _userId = userId;
        _store = store;
        _state = new PresenceState
        {
            IsOnline = false,
            ConnectionCount = 0,
            LastSeenAtUtc = null
        };

        ReceiveAsync<UserConnectedActorCommand>(HandleUserConnected);
        ReceiveAsync<UserDisconnectedActorCommand>(HandleUserDisconnected);
        Receive<GetUserPresenceActorCommand>(HandleGetPresence);
    }

    private async Task HandleUserConnected(UserConnectedActorCommand message)
    {
        _state.ConnectionCount++;

        if (_state.ConnectionCount == 1)
        {
            _state.IsOnline = true;
            _state.LastSeenAtUtc = null;

            var userOnlineEvent = new UserOnline(_userId, DateTime.UtcNow);
            
            await using var session = _store.LightweightSession();
            session.Events.Append(_userId, userOnlineEvent);
            await session.SaveChangesAsync();

            Context.System.EventStream.Publish(userOnlineEvent);
        }
    }

    private async Task HandleUserDisconnected(UserDisconnectedActorCommand message)
    {
        _state.ConnectionCount = Math.Max(0, _state.ConnectionCount - 1);

        if (_state.ConnectionCount == 0)
        {
            _state.IsOnline = false;
            _state.LastSeenAtUtc = DateTime.UtcNow;

            var userOfflineEvent = new UserOffline(_userId, _state.LastSeenAtUtc.Value);

            await using var session = _store.LightweightSession();
            session.Events.Append(_userId, userOfflineEvent);
            await session.SaveChangesAsync();

            Context.System.EventStream.Publish(userOfflineEvent);
        }
    }

    private void HandleGetPresence(GetUserPresenceActorCommand message)
    {
        Sender.Tell(_state);
    }
}
