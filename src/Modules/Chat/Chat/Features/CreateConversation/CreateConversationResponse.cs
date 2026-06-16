using Identity.Contracts.DTOs;

namespace Chat.Features.CreateConversation;

/// <summary>
/// Response returned after a conversation is successfully created or found.
/// </summary>
public sealed record CreateConversationResponse(
    Guid ConversationId,
    UserDto ParticipantB);
