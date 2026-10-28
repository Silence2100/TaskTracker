using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();

    Task<List<Project>> GetByMemberIdAsync(Guid memberId);

    Task<Project?> GetByIdAsync(Guid id);

    Task<Project> CreateAsync(Project project);
}