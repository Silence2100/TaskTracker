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
                var dto = task.ToDto();
                dto.ProjectName = project.Name;
                return dto;
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

        return Result<TaskDto>.Success(ToTaskDto(task, project.Name));
    }

    public async Task<Result<TaskDto>> CreateAsync(Guid projectId, CreateTaskDto dto, Guid authorId)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length > 200)
            return Result<TaskDto>.Failure(TaskErrors.InvalidTitle);

        if (dto.Description?.Length > 2000)
            return Result<TaskDto>.Failure(TaskErrors.InvalidDescription);

        if (dto.AssignedUserId == Guid.Empty)
            return Result<TaskDto>.Failure(TaskErrors.InvalidAssignee);

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

        return Result<TaskDto>.Success(ToTaskDto(task, project.Name));
    }

    public async Task<Result<bool>> DeleteAsync(Guid projectId, int taskId, Guid userId)
    {
        var projectResult = await GetProjectForEditAsync(projectId, userId);

        if (projectResult.IsFailure)
            return Result<bool>.Failure(projectResult.Error);

        var project = projectResult.Value;

        if (!project.RemoveTask(taskId))
            return Result<bool>.Failure(TaskErrors.NotFound);

        await _projectRepository.SaveChangesAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<TaskDto>> UpdateDetailsAsync(Guid projectId, int taskId, UpdateTaskDto dto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || dto.Title.Trim().Length > 200)
            return Result<TaskDto>.Failure(TaskErrors.InvalidTitle);

        if (dto.Description?.Length > 2000)
            return Result<TaskDto>.Failure(TaskErrors.InvalidDescription);

        var projectResult = await GetProjectForEditAsync(projectId, userId);

        if (projectResult.IsFailure)
            return Result<TaskDto>.Failure(projectResult.Error);

        var project = projectResult.Value;

        var task = project.UpdateTaskDetails(taskId, dto.Title, dto.Description);

        if (task is null)
            return Result<TaskDto>.Failure(TaskErrors.NotFound);

        await _projectRepository.SaveChangesAsync();

        return Result<TaskDto>.Success(ToTaskDto(task, project.Name));
    }

    public async Task<Result<TaskDto>> AssignMemberAsync(Guid projectId, int taskId, AssignTaskMemberDto dto, Guid userId)
    {
        if (dto.AssignedUserId == Guid.Empty)
            return Result<TaskDto>.Failure(TaskErrors.InvalidAssignee);

        var projectResult = await GetProjectForEditAsync(projectId, userId);

        if (projectResult.IsFailure)
            return Result<TaskDto>.Failure(projectResult.Error);

        var project = projectResult.Value;

        if (project.FindTask(taskId) is null)
            return Result<TaskDto>.Failure(TaskErrors.NotFound);

        if (dto.AssignedUserId is not null)
        {
            var isMember = await _memberRepository.IsMemberAsync(projectId, dto.AssignedUserId.Value);

            if (!isMember)
                return Result<TaskDto>.Failure(TaskErrors.AssigneeNotProjectMember);
        }

        var task = project.AssignTaskMember(taskId, dto.AssignedUserId)!;

        await _projectRepository.SaveChangesAsync();

        return Result<TaskDto>.Success(ToTaskDto(task, project.Name));
    }

    public async Task<Result<TaskDto>> UpdateStatusAsync(Guid projectId, int taskId, UpdateTaskStatusDto dto, Guid userId)
    {
        if (!Enum.IsDefined(typeof(TaskItemStatus), dto.Status))
            return Result<TaskDto>.Failure(TaskErrors.InvalidStatus);

        var projectResult = await GetProjectForEditAsync(projectId, userId);

        if (projectResult.IsFailure)
            return Result<TaskDto>.Failure(projectResult.Error);

        var project = projectResult.Value;

        var task = project.ChangeTaskStatus(taskId, dto.Status);

        if (task is null)
            return Result<TaskDto>.Failure(TaskErrors.NotFound);

        await _projectRepository.SaveChangesAsync();

        return Result<TaskDto>.Success(ToTaskDto(task, project.Name));
    }

    private async Task<Result<Project>> GetProjectForEditAsync(Guid projectId, Guid userId)
    {
        var project = await _projectRepository.GetForUpdateAsync(projectId);

        if (project is null)
            return Result<Project>.Failure(ProjectErrors.NotFound);

        var isMember = await _memberRepository.IsMemberAsync(projectId, userId);

        if (!isMember)
            return Result<Project>.Failure(TaskErrors.AccessDenied);

        return Result<Project>.Success(project);
    }

    private static TaskDto ToTaskDto(TaskItem task, string projectName)
    {
        var dto = task.ToDto();
        dto.ProjectName = projectName;

        return dto;
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