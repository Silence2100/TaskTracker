using TaskTracker.Application.Common;
using TaskTracker.Application.DTOs.Projects;

namespace TaskTracker.Application.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync();

    Task<ProjectDto?> GetByIdAsync(Guid id);

    Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto, Guid userId);

    Task<Result<bool>> DeleteAsync(Guid projectId, Guid userId);

    Task<Result<List<ProjectMemberDto>>> GetMembers(Guid userId, Guid projectId);

    Task<Result<ProjectMemberDto>> AddMemberAsync(Guid projectId, Guid userId, Guid currentUserId);

    Task<Result<bool>> RemoveMemberAsync(Guid projectId, Guid userId, Guid currentUserId);

    Task<Result<ProjectMemberDto>> UpdateMemberRoleAsync(Guid projectId, UpdateProjectMemberRoleDto dto, Guid currentUserId);
}