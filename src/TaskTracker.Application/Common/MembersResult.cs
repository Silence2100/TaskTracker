using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Common;

public class MembersResult
{
    public Guid? ProjectId { get; set; }
    public bool CanGetMembers { get; set; }
    public List<ProjectMemberDto>? Members { get; set; }
}