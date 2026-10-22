using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllAsync();
    Task<List<TaskItem>> GetByProjectMemberIdAsync(Guid memberId);
    Task<TaskItem?> GetByIdAsync(int taskId);
    Task<TaskItem> CreateAsync(Guid projectId, TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    Task<TaskItem?> GetAccessibleByIdAsync(int taskId, Guid userId);
    Task<List<TaskItem>> GetByProjectIdAsync(Guid projectId);
}