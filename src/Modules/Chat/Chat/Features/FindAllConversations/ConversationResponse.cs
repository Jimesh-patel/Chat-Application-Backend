using Identity.Contracts.DTOs;

namespace Chat.Features.FindAllConversations;

/// <summary>
/// Response DTO for a conversation, including full details of the other participant.
/// </summary>
public sealed record ConversationResponse(
    Guid ConversationId,
    DateTime CreatedAtUtc,
    DateTime LastMessageAtUtc,
    int UnseenMessageCount,
    UserDto ParticipantB);
