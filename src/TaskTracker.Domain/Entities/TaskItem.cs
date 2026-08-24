using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public TaskItemStatus Status { get; set; }
    public Guid? AssignedUserId { get; set; } 
    public Guid AuthorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}