using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();

    Task<List<Project>> GetByMemberIdAsync(Guid memberId);

    Task<Project?> GetByIdAsync(Guid id);

    Task<Project> CreateAsync(Project project);

    Task<List<ProjectMember>> GetMembersAsync(Guid projectId);

    Task<bool> IsMemberAsync(Guid projectId, Guid userId);

    Task<ProjectMember?> GetProjectMember(Guid userId, Guid projectId);

    Task<ProjectRole> GetUserRoleAsync(Guid userId, Guid projectId);

    Task<bool> HasOwnerRoleAsync(Guid userId);
}