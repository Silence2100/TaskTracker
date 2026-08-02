using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public static class ProjectMemberMapper
{
    public static ProjectMemberDto ToDto(this ProjectMember member)
    {
        return new ProjectMemberDto
        {
            UserId = member.UserId,
            UserName = member.User.Name,
            UserEmail = member.User.Email.Value,
            Role = member.Role
        };
    }
}