using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class ProjectMember
{
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public ProjectRole Role { get; set; }
}