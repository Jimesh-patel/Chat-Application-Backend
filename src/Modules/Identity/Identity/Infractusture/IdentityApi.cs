using Identity.Contracts.APIs;
using Identity.Contracts.DTOs;
using Identity.Domain;
using Identity.ReadModels;
using Marten;
using Platform.Common.Results;

namespace Identity.Infractusture;

internal class IdentityApi(IQuerySession querySession) : IIdentityApi
{
    public async Task<Result<UserDto>> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await querySession.Query<UserReadModel>()
            .FirstOrDefaultAsync(u => u.Id == new UserId(userId), token: cancellationToken);

        if (user is null)
        {
            return Result<UserDto>.Failure(new Error("Identity.UserNotFound", "User Not Found"));
        }

        return Result<UserDto>.Success(new UserDto(
            user.Id.Value,
            user.Email,
            user.Username,
            user.DisplayName));
    }
}
