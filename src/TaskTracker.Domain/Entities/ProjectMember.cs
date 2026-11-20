using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class ProjectMember
{
    public Guid UserId { get; private set; }

    public Guid ProjectId { get; private set; }

    public ProjectRole Role { get; private set; }

    private ProjectMember() { }

    internal ProjectMember(Guid projectId, Guid userId, ProjectRole role)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(projectId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        ValidateRole(role);

        ProjectId = projectId;
        UserId = userId;
        Role = role;
    }

    internal void ChangeRole(ProjectRole role)
    {
        ValidateRole(role);

        Role = role;
    }

    private static void ValidateRole(ProjectRole role)
    {
        if (!Enum.IsDefined(role))
            throw new ArgumentOutOfRangeException(nameof(role), role, "Invalid project role.");
    }
}