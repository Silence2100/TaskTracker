namespace TaskTracker.Application.DTOs.Projects;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TasksCount { get; set; }
    public int MembersCount { get; set; }
}