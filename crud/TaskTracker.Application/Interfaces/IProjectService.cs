using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<ProjectDto?> CreateAsync(CreateProjectDto dto);
    Task<List<ProjectMemberDto>?> GetMembersAsync(Guid projectId);
}