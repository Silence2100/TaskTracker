using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Users;

public class UpdateUserDto
{
    public string? Login { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public UserRole? Role { get; set; }
    public bool IsBlock { get; set; }
}