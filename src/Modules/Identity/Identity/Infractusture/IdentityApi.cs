using Identity.Contracts.APIs;
using Identity.Contracts.DTOs;

namespace Identity.Infractusture;

internal class IdentityApi : IIdentityApi
{
    Task<UserDto?> IIdentityApi.GetUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
