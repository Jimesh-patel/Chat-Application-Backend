using Identity.Contracts.DTOs;
using Platform.Common.Results;

namespace Identity.Contracts.APIs;

public interface IIdentityApi
{
    Task<Result<UserDto>> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}