using Chat.ReadModels;
using Marten;
using Platform.Common.Results;
using Platform.Contracts.Queries;

namespace Chat.Features.GetMessages;

/// <summary>
/// Handles <see cref="GetMessagesQuery"/> by querying the standalone MessageReadModel documents from Marten.
/// </summary>
internal sealed class GetMessagesHandler(IQuerySession querySession)
    : IQueryHandler<GetMessagesQuery, IReadOnlyList<MessageDto>>
{
    public async Task<Result<IReadOnlyList<MessageDto>>> Handle(
        GetMessagesQuery query,
        CancellationToken cancellationToken)
    {
        var messages = await querySession
            .Query<MessageReadModel>()
            .Where(m => m.ConversationId == query.ConversationId)
            .OrderBy(m => m.SentAtUtc)
            .ToListAsync(cancellationToken);

        var dtos = messages.Select(m => new MessageDto(
            m.Id,
            m.ConversationId,
            m.SenderId,
            m.Content,
            m.SentAtUtc)).ToList();

        return Result<IReadOnlyList<MessageDto>>.Success(dtos);
    }
}
