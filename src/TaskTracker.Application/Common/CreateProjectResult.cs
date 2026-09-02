using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Common;

public class CreateProjectResult
{
    public bool CanCreateProject { get; set; }

    public ProjectDto? Project { get; set; }
}