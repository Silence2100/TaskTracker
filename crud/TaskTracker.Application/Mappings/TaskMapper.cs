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
            ProjectId = task.ProjectId,
            ProjectName = task.Project.Name,
            Title = task.Title,
            Description = task.Description,
            Deadline = task.Deadline,
            Status = task.Status,
            AssignedUserId = task.AssignedUserId,
            AssignedUserName = task.AssignedUser?.Name,
            AuthorId = task.AuthorId,
            AuthorName = task.Author.Name,
            CreatedAt = task.CreatedAt,
            UpdateAt = task.UpdateAt
        };
    }
}