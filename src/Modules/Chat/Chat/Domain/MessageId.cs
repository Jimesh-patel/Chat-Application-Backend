namespace Chat.Domain;

/// <summary>
/// Strongly-typed identifier for a message within a <see cref="Conversation"/>.
/// </summary>
public readonly record struct MessageId(Guid Value)
{
    public static MessageId New() => new(Guid.NewGuid());

    public static implicit operator Guid(MessageId id) => id.Value;

    public static implicit operator MessageId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
