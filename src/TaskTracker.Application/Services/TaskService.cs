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
    private readonly IProjectRepository _projectRepository;
    private readonly IMemberRepository _memberRepository;

    public TaskService(IProjectRepository projectRepository, IMemberRepository memberRepository)
    {
        _projectRepository = projectRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Result<List<TaskDto>>> GetByProjectAsync(Guid projectId, Guid userId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return Result<List<TaskDto>>.Failure(ProjectErrors.NotFound);

        var isMember = await _memberRepository.IsMemberAsync(projectId, userId);

        if (!isMember)
            return Result<List<TaskDto>>.Failure(TaskErrors.AccessDenied);

        var tasks = project.Tasks
            .OrderByDescending(task => task.CreatedAt)
            .Select(task =>
            {
                var taskDto = task.ToDto();
                taskDto.ProjectName = project.Name;
                return taskDto;
            })
            .ToList();

        return Result<List<TaskDto>>.Success(tasks);
    }

    public async Task<Result<TaskDto>> GetByIdAsync(Guid projectId, int taskId, Guid userId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return Result<TaskDto>.Failure(ProjectErrors.NotFound);

        var isMember = await _memberRepository.IsMemberAsync(projectId, userId);

        if (!isMember)
            return Result<TaskDto>.Failure(TaskErrors.AccessDenied);

        var task = project.FindTask(taskId);

        if (task is null)
            return Result<TaskDto>.Failure(TaskErrors.NotFound);

        var taskDto = task.ToDto();
        taskDto.ProjectName = project.Name;

        return Result<TaskDto>.Success(taskDto);
    }

    public async Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskDto dto, Guid authorId)
    {
        var project = await _projectRepository.GetForUpdateAsync(projectId);

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

        project.AddTask(task);

        await _projectRepository.SaveChangesAsync();

        var taskDto = task.ToDto();
        taskDto.ProjectName = project.Name;

        return Result<TaskDto>.Success(taskDto);
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