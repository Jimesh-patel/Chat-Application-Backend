namespace Platform.Akka;

public sealed class AkkaOptions
{
    public const string SectionName = "Akka";

    public string ActorSystemName { get; init; } = "ChatApp";
}
