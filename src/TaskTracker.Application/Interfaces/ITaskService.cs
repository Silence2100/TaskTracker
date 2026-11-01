using TaskTracker.Application.Common;
using TaskTracker.Application.DTOs.Tasks;

namespace TaskTracker.Application.Interfaces;

public interface ITaskService
{
    Task<Result<List<TaskDto>>> GetByProjectAsync(Guid projectId, Guid userId);

    Task<Result<TaskDto>> GetByIdAsync(Guid projectId, int taskId, Guid userId);

    Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskDto dto, Guid authorId);

    Task<Result<bool>> DeleteAsync(Guid projectId, int taskId, Guid userId);

    Task<Result<TaskDto>> UpdateDetailsAsync(Guid projectId, int taskId, UpdateTaskDto dto, Guid userId);

    Task<Result<TaskDto>> AssignMemberAsync(Guid projectId, int taskId, AssignTaskMemberDto dto, Guid userId);

    Task<Result<TaskDto>> UpdateStatusAsync(Guid projectId, int taskId, UpdateTaskStatusDto dto, Guid userId);
}