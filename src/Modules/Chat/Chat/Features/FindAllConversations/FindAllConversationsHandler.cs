using Chat.Domain;
using Chat.ReadModels;
using Identity.Contracts.APIs;
using Marten;
using Platform.Common.Results;
using Platform.Contracts.Queries;

namespace Chat.Features.FindAllConversations;

internal sealed class FindAllConversationsHandler(
    IQuerySession querySession,
    IIdentityApi identityApi)
    : IQueryHandler<FindAllConversationsQuery, IReadOnlyList<ConversationResponse>>
{
    public async Task<Result<IReadOnlyList<ConversationResponse>>> Handle(
        FindAllConversationsQuery query,
        CancellationToken cancellationToken)
    {
        var conversations = await querySession
            .Query<ConversationReadModel>()
            .Where(c => c.ParticipantA == query.UserId || c.ParticipantB == query.UserId)
            .ToListAsync(cancellationToken);

        var conversationIds = conversations.Select(c => c.Id.Value).ToList();

        var unseenMessages = await querySession
            .Query<MessageReadModel>()
            .Where(m => conversationIds.Contains(m.ConversationId) && 
                        m.RecipientId == query.UserId && 
                        m.Status != MessageStatus.Seen)
            .Select(m => m.ConversationId)
            .ToListAsync(cancellationToken);

        var unseenCounts = unseenMessages.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

        var responses = new List<ConversationResponse>();

        foreach (var conv in conversations)
        {
            var otherParticipantId = conv.ParticipantA == query.UserId
                ? conv.ParticipantB
                : conv.ParticipantA;

            var otherParticipant = await identityApi.GetUserAsync(otherParticipantId, cancellationToken);
            if (otherParticipant is null)
            {
                continue; // Or handle missing user scenario
            }

            var unseenCount = unseenCounts.TryGetValue(conv.Id, out var count) ? count : 0;

            responses.Add(new ConversationResponse(
                conv.Id,
                conv.CreatedAtUtc,
                conv.LastMessageAtUtc,
                unseenCount,
                otherParticipant.Value!));
        }

        // Sort by last message time descending
        return Result<IReadOnlyList<ConversationResponse>>.Success(
            [.. responses.OrderByDescending(r => r.LastMessageAtUtc)]);
    }
}
