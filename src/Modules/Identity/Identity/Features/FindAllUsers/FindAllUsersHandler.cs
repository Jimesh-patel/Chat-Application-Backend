using Identity.ReadModels;
using Marten;
using Platform.Common.Results;
using Platform.Contracts.Queries;

namespace Identity.Features.FindAllUsers;

/// <summary>
/// Handles the <see cref="FindAllUsersQuery"/> by querying the read models.
/// </summary>
internal sealed class FindAllUsersHandler(IQuerySession querySession)
    : IQueryHandler<FindAllUsersQuery, IReadOnlyList<UserResponse>>
{
    public async Task<Result<IReadOnlyList<UserResponse>>> Handle(
        FindAllUsersQuery query,
        CancellationToken cancellationToken)
    {
        var users = await querySession
            .Query<UserReadModel>()
            .Select(u => new UserResponse(
                u.Id,
                u.Email,
                u.Username,
                u.DisplayName))
            .ToListAsync(cancellationToken);

        return Result<IReadOnlyList<UserResponse>>.Success(users);
    }
}
