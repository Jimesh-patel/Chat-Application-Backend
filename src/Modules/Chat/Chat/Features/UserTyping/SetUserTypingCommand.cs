using Platform.Contracts.Commands;
using System.Text.Json.Serialization;

namespace Chat.Features.UserTyping;

public sealed record SetUserTypingCommand(
    Guid ConversationId,
    Guid UserId,
    bool IsTyping) : ICommand<Guid>;
