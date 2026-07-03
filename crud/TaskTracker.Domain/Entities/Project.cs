namespace TaskTracker.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TaskItem> Tasks { get; set; } = new();
    public List<ProjectMember> Members { get; set; } = new();
}