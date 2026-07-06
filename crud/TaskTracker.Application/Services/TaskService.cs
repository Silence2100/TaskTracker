using AutoMapper;
using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository, IMapper mapper)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
    }

    public async Task<List<TaskDto>> GetAllAsync()
    {
        var tasks = await _taskRepository.GetAllAsync();

        return _mapper.Map<List<TaskDto>>(tasks);
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _taskRepository.GetByIdAsync(id);

        if (task is null)
            return null;

        return _mapper.Map<TaskDto>(task);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto dto)
    {
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
            AuthorId = dto.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdateAt = null
        };

        var createdTask = await _taskRepository.CreateAsync(task);

        return _mapper.Map<TaskDto>(createdTask);
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