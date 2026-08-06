using TaskTracker.Application.DTOs.Tasks;

namespace TaskTracker.Application.Interfaces;

public interface ITaskService
{
    Task<List<TaskDto>> GetAllAsync();
    Task<List<TaskDto>> GetByProjectMemberIdAsync(Guid memberId);
    Task<TaskDto?> GetByIdAsync(Guid id);
    Task<TaskDto?> CreateAsync(CreateTaskDto dto, Guid authorId);
    Task<bool> UpdateAsync(Guid id, UpdateTaskDto dto);
    Task<bool> DeleteAsync(Guid id);
}