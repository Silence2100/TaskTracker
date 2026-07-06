using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Projects;

public class ProjectMemberDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public ProjectRole Role { get; set; }
}