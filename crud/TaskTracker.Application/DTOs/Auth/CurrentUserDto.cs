using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Auth;

public sealed class CurrentUserDto
{
    public Guid Id { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public UserRole Role { get; init; }
}