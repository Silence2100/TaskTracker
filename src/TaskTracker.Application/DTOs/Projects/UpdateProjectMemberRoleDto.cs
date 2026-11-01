using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Projects;

public class UpdateProjectMemberRoleDto
{
    public Guid UserId { get; set; }

    public ProjectRole Role { get; set; }
}