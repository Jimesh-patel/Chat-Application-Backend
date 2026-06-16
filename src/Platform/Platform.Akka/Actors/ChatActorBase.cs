using Akka.Actor;
using Serilog;

namespace Platform.Akka.Actors;

public abstract class ChatActorBase : ReceiveActor
{
    protected virtual string ActorName => GetType().Name;

    protected sealed override void PreStart()
    {
        base.PreStart();
        LogInfo("Actor started at {Path}", Self.Path);
        OnPreStart();
    }

    protected sealed override void PostStop()
    {
        OnPostStop();
        LogInfo("Actor stopped at {Path}", Self.Path);
        base.PostStop();
    }

    protected virtual void OnPreStart() { }

    protected virtual void OnPostStop() { }

    protected void LogInfo(string messageTemplate, params object?[] args)
        => Log.ForContext(nameof(ActorName), ActorName)
               .Information("[{ActorName}] " + messageTemplate, [ActorName, .. args]);

    protected void LogWarning(string messageTemplate, params object?[] args)
        => Log.ForContext(nameof(ActorName), ActorName)
               .Warning("[{ActorName}] " + messageTemplate, [ActorName, .. args]);

    protected void LogError(Exception? ex, string messageTemplate, params object?[] args)
        => Log.ForContext(nameof(ActorName), ActorName)
               .Error(ex, "[{ActorName}] " + messageTemplate, [ActorName, .. args]);
}
