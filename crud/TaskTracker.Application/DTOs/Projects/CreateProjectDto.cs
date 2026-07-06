namespace TaskTracker.Application.DTOs.Projects;

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
}