namespace Presence.Domain;

public sealed class PresenceState
{
    public bool IsOnline { get; set; }

    public int ConnectionCount { get; set; }

    public DateTime? LastSeenAtUtc { get; set; }
}
