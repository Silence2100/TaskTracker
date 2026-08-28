using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappings;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskDto>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();

        return tasks
            .Select(task => task.ToDto())
            .ToList();
    }

    public async Task<List<TaskDto>> GetByProjectMemberIdAsync(Guid memberId)
    {
        var tasks = await _taskRepository.GetByProjectMemberIdAsync(memberId);

        return tasks
            .Select(task => task.ToDto())
            .ToList();
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        return task?.ToDto();
    }

    public async Task<TaskDto?> CreateAsync(CreateTaskDto dto, Guid authorId)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim(),
            Deadline = NormalizeDateTime(dto.Deadline),
            Status = TaskItemStatus.Todo,
            AssignedUserId = dto.AssignedUserId,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow,
            UpdateAt = null
        };

        var createdTask = await _taskRepository.CreateAsync(task);

        return createdTask.ToDto();
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            return false;

        task.Title = dto.Title.Trim();
        task.Description = string.IsNullOrWhiteSpace(dto.Description)
            ? null
            : dto.Description.Trim();
        task.Deadline = NormalizeDateTime(dto.Deadline);
        task.Status = dto.Status;
        task.AssignedUserId = dto.AssignedUserId;
        task.UpdateAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            return false;

        await _taskRepository.DeleteAsync(task);

        return true;
    }

    private static DateTime? NormalizeDateTime(DateTime? dateTime)
    {
        if (dateTime is null)
            return null;

        if (dateTime.Value.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);

        return dateTime.Value.ToUniversalTime();
    }
}