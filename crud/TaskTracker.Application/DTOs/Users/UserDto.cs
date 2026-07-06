namespace TaskTracker.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}