using TaskTracker.Application.Common;
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

    public async Task<CreateTaskResult> CreateAsync(Guid projectId, CreateTaskDto dto, Guid authorId)
    {
        var result = new CreateTaskResult();

        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return result;

        result.ProjectExists = true;

        var isAuthorMember = await _projectRepository.IsMemberAsync(projectId, authorId);

        if (!isAuthorMember)
            return result;

        result.AuthorIsProjectMember = true;

        if (dto.AssignedUserId is not null)
        {
            var isAssigneeMember = await _projectRepository.IsMemberAsync(projectId, dto.AssignedUserId.Value);

            if (!isAssigneeMember)
            {
                result.AssigneeIsProjectMember = false;

                return result;
            }
        }

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
            CreatedAt = DateTime.UtcNow
        };

        var createdTask = await _taskRepository.CreateAsync(projectId, task);

        result.Task = createdTask.ToDto();

        return result;
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