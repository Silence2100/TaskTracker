using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Errors;
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
    private readonly IMemberRepository _memberRepository;

    public TaskService(ITaskRepository taskRepository, IProjectRepository projectRepository, IMemberRepository memberRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _memberRepository = memberRepository;
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

    public async Task<TaskDto?> GetByIdAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        return task?.ToDto();
    }

    public async Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskDto dto, Guid authorId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return Result<TaskDto>.Failure(ProjectErrors.NotFound);

        var isAuthorMember = await _memberRepository.IsMemberAsync(projectId, authorId);

        if (!isAuthorMember)
            return Result<TaskDto>.Failure(TaskErrors.AuthorNotProjectMember);

        if (dto.AssignedUserId is not null)
        {
            var isAssigneeMember = await _memberRepository.IsMemberAsync(projectId, dto.AssignedUserId.Value);

            if (!isAssigneeMember)
                return Result<TaskDto>.Failure(TaskErrors.AssigneeNotProjectMember);
        }

        var task = new TaskItem
        {
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

        return Result<TaskDto>.Success(createdTask.ToDto());
    }

    public async Task<bool> UpdateAsync(int id, UpdateTaskDto dto)
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

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            return false;

        await _taskRepository.DeleteAsync(task);

        return true;
    }

    public async Task<IReadOnlyList<TaskItem>?> GetByProjectAsync(Guid projectId, Guid? userId)
    {
        var isMember = await _memberRepository.IsMemberAsync(projectId, userId);

        if (!isMember)
            return null;

        var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

        return tasks;
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