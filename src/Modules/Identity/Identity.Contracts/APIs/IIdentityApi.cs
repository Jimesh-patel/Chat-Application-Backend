using Identity.Contracts.DTOs;

namespace Identity.Contracts.APIs;

public interface IIdentityApi
{
    Task<UserDto?> GetUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}