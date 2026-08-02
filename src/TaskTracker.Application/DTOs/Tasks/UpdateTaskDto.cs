using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.DTOs.Tasks;

public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public TaskItemStatus Status { get; set; }
    public Guid? AssignedUserId { get; set; }
}