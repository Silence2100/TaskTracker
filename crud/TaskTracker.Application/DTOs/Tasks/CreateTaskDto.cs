namespace TaskTracker.Application.DTOs.Tasks;

public class CreateTaskDto
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public Guid? AssignedUserId { get; set; }
    public Guid AuthorId { get; set; }
}