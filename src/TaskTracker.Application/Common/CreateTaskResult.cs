using TaskTracker.Application.DTOs.Tasks;

namespace TaskTracker.Application.Common;

public class CreateTaskResult
{
    public bool ProjectExists { get; set; }

    public bool AuthorIsProjectMember { get; set; }

    public bool AssigneeIsProjectMember { get; set; } = true;

    public TaskDto? Task { get; set; }
}