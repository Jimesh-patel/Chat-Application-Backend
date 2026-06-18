using Akka.Actor;

namespace Presence.Infrastructure.Actors;

public sealed record UserConnectedActorCommand(string ConnectionId);

public sealed record UserDisconnectedActorCommand(string ConnectionId);

public sealed record GetUserPresenceActorCommand(Guid UserId);

public sealed record GetUserPresenceActor(Guid UserId);
