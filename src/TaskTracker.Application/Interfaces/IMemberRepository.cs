using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Interfaces;

public interface IMemberRepository
{
    Task<List<ProjectMember>> GetByProjectIdAsync(Guid projectId);

    Task<ProjectMember?> GetAsync(Guid projectId, Guid userId);

    Task<bool> IsMemberAsync(Guid projectId, Guid? userId);

    Task<ProjectRole?> GetRoleAsync(Guid projectId, Guid userId);

    Task<bool> HasOwnerRoleAsync(Guid userId);

    Task AddAsync(ProjectMember member);

    Task UpdateAsync(ProjectMember member);

    Task DeleteAsync(ProjectMember member);
}