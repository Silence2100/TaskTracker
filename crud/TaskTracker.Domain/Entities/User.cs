namespace TaskTracker.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<ProjectMember> ProjectMembers { get; set; } = new();
    public List<TaskItem> AuthoredTasks { get; set; } = new();
    public List<TaskItem> AssignedTasks { get; set; } = new();
}