using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TaskItem> Tasks { get; set; } = [];
    public List<ProjectMember> Members { get; set; } = [];

    public bool TryGetMembers(Guid? userId, out List<ProjectMember> members)
    {
        var user = Members.FirstOrDefault(member => member.UserId == userId);

        if (user is null || user.Role != ProjectRole.Owner)
        {
            members = [];

            return false;
        }

        members = [.. Members];

        return true;
    }
}