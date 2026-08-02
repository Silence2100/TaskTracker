using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public static class ProjectMapper
{
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            TasksCount = project.Tasks.Count,
            MembersCount = project.Members.Count
        };
    }
}