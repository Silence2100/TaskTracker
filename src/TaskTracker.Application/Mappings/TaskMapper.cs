using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public static class TaskMapper
{
    public static TaskDto ToDto(this TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            Status = task.Status,
            AssignedUserId = task.AssignedUserId,
            AuthorId = task.AuthorId,
            CreatedAt = task.CreatedAt,
            UpdateAt = task.UpdateAt
        };
    }
}