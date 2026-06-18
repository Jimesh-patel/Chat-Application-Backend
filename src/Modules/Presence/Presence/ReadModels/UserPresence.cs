namespace Presence.ReadModels;

public sealed class UserPresence
{
    public Guid Id { get; set; } // user_id
    public bool IsOnline { get; set; }
    public DateTime? LastSeenAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
