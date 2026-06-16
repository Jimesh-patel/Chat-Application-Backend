namespace Platform.Realtime.Abstractions;

public interface IRealtimeNotifier
{
    Task SendToUserAsync(
        Guid userId,
        string eventName,
        object payload,
        CancellationToken cancellationToken = default);
}
