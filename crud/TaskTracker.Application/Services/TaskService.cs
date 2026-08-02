using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappings;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<List<TaskDto>> GetAllAsync(Guid currentUserId, UserRole currentUserRole)
    {
        var tasks = currentUserRole == UserRole.Admin
            ? await _taskRepository.GetAllAsync()
            : await _taskRepository.GetAllForUserAsync(currentUserId);

        return tasks.Select(task => task.ToDto()).ToList();
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            return null;

        var canAccess = await CanAccessProjectAsync(task.ProjectId, currentUserId, currentUserRole);

        if (!canAccess)
            return null;

        return task.ToDto();
    }

    public async Task<TaskDto?> CreateAsync(CreateTaskDto dto, Guid authorId, UserRole currentUserRole)
    {
        var canAccess = await CanAccessProjectAsync(dto.ProjectId, authorId, currentUserRole);

        if (!canAccess)
            return null;

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = dto.ProjectId,
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

    public async Task<bool> UpdateAsync(Guid id, UpdateTaskDto dto, Guid currentUserId, UserRole currentUserRole)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            return false;

        var canAccess = await CanAccessProjectAsync(task.ProjectId, currentUserId, currentUserRole);

        if (!canAccess)
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

    public async Task<bool> DeleteAsync(Guid id, Guid currentUserId, UserRole currentUserRole)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            return false;

        var canAccess = await CanAccessProjectAsync(task.ProjectId, currentUserId, currentUserRole);

        if (!canAccess)
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

    private async Task<bool> CanAccessProjectAsync(Guid projectId, Guid userId, UserRole userRole)
    {
        if (userRole == UserRole.Admin)
            return true;

        return await _projectRepository.IsMemberAsync(projectId, userId);
    }
}