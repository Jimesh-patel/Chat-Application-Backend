namespace Identity.Contracts.DTOs;

public sealed record UserDto(
    Guid Id,
    string Email,
    string Username,
    string DisplayName);