using TaskTracker.Application.Common;
using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<MembersResult> GetMembers(Guid? userId, Guid projectId);
    Task<List<ProjectDto>> GetByMemberIdAsync(Guid memberId);
    Task<ProjectDto?> CreateAsync(CreateProjectDto dto, Guid ownerUserId);
}