using TaskTracker.Application.Common;
using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();
    Task<List<ProjectDto>> GetByMemberIdAsync(Guid memberId);
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<ProjectDto?> CreateAsync(CreateProjectDto dto, Guid ownerUserId);
    Task<List<ProjectMemberDto>?> GetMembersAndOwnerAsync(Guid projectId);
    Task<AccessResult?> GetAccessResult(Guid ownerUserId, Guid projectId);
}