using TaskTracker.Application.DTOs.Users;

namespace TaskTracker.Application.DTOs.Auth;

public sealed class AuthResponseDto
{
    public string TokenType { get; init; } = "Bearer";
    public string AccessToken { get; init; } = string.Empty;
    public UserDto User { get; init; } = null!;
}