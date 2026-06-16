namespace Chat.Domain;

/// <summary>
/// Strongly-typed identifier for a <see cref="Conversation"/> aggregate.
/// </summary>
public readonly record struct ConversationId(Guid Value)
{
    public static ConversationId New() => new(Guid.NewGuid());

    public static implicit operator Guid(ConversationId id) => id.Value;

    public static implicit operator ConversationId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
