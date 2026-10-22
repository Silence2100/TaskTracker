using TaskTracker.Application.Common;
using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Interfaces;

public interface ITaskService
{
    Task<List<TaskDto>> GetAllAsync();
    Task<List<TaskDto>> GetByProjectMemberIdAsync(Guid memberId);
    Task<TaskDto?> GetByIdAsync(int id);
    Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskDto dto, Guid authorId);
    Task<bool> UpdateAsync(int id, UpdateTaskDto dto);
    Task<bool> DeleteAsync(int id);
    Task<IReadOnlyList<TaskItem>?> GetByProjectAsync(Guid projectId, Guid userId);
}