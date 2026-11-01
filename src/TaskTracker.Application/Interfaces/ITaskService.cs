using TaskTracker.Application.Common;
using TaskTracker.Application.DTOs.Tasks;

namespace TaskTracker.Application.Interfaces;

public interface ITaskService
{
    Task<Result<List<TaskDto>>> GetByProjectAsync(Guid projectId, Guid userId);

    Task<Result<TaskDto>> GetByIdAsync(Guid projectId, int taskId, Guid userId);

    Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskDto dto, Guid authorId);
}