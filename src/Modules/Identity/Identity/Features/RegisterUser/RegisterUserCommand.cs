using Platform.Contracts.Commands;

namespace Identity.Features.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Username,
    string Password)
    : ICommand<Guid>;