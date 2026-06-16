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

            responses.Add(new ConversationResponse(
                conv.Id,
                conv.CreatedAtUtc,
                conv.LastMessageAtUtc,
                conv.MessageCount,
                otherParticipant.Value));
        }

        // Sort by last message time descending
        return Result<IReadOnlyList<ConversationResponse>>.Success(
            [.. responses.OrderByDescending(r => r.LastMessageAtUtc)]);
    }
}
