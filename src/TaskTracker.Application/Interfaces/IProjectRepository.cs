using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();

    Task<Project?> GetByIdAsync(Guid id);

    Task<Project?> GetForUpdateAsync(Guid id);

    Task<Project> CreateAsync(Project project);

    Task<bool> DeleteAsync(Guid id);

    Task SaveChangesAsync();
}