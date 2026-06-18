using Marten.Events.Projections;
using Presence.Contracts.Events;
using Presence.ReadModels;

namespace Presence.Projections;

public sealed partial class PresenceProjection : MultiStreamProjection<UserPresence, Guid>
{
    public PresenceProjection()
    {
        Identity<UserOnline>(x => x.UserId);
        Identity<UserOffline>(x => x.UserId);
    }

    public void Apply(UserOnline e, UserPresence current)
    {
        current.Id = e.UserId;
        current.IsOnline = true;
        current.UpdatedAtUtc = e.OccurredAtUtc;
    }

    public void Apply(UserOffline e, UserPresence current)
    {
        current.Id = e.UserId;
        current.IsOnline = false;
        current.LastSeenAtUtc = e.LastSeenAtUtc;
        current.UpdatedAtUtc = DateTime.UtcNow;
    }
}
