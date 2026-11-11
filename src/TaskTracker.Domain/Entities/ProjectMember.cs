using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class ProjectMember
{
    public Guid UserId { get; private set; }

    public Guid ProjectId { get; private set; }

    public ProjectRole Role { get; private set; }

    private ProjectMember() {}

    internal ProjectMember(Guid projectId, Guid userId, ProjectRole role)
    {
        ProjectId = projectId;
        UserId = userId;
        Role = role;
    }

    internal void ChangeRole(ProjectRole role)
    {
        Role = role;
    }
}