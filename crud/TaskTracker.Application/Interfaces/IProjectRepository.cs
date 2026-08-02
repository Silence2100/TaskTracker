using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<List<Project>> GetAllForUserAsync(Guid userId);
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project> CreateAsync(Project project);
    Task<List<ProjectMember>> GetMembersAsync(Guid projectId);
    Task<bool> IsMemberAsync(Guid projectId, Guid userId);
}