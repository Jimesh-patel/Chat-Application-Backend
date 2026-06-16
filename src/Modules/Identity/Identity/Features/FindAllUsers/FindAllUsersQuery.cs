using Platform.Contracts.Queries;

namespace Identity.Features.FindAllUsers;

/// <summary>
/// Query to retrieve a list of all registered users.
/// </summary>
public sealed record FindAllUsersQuery : IQuery<IReadOnlyList<UserResponse>>;
