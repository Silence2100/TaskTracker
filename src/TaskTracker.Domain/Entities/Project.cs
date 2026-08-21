namespace TaskTracker.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<User> Users { get; set; } = [];
    public List<TaskItem> Tasks { get; set; } = [];
    public List<ProjectMember> Members { get; set; } = [];
}