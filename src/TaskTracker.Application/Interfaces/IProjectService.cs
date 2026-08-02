using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Interfaces;

public interface IProjectService
{
    Task<List<ProjectDto>> GetAllAsync(Guid currentUserId, UserRole currentUserRole);
    Task<ProjectDto?> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole);
    Task<ProjectDto?> CreateAsync(CreateProjectDto dto, Guid ownerUserId);
    Task<List<ProjectMemberDto>?> GetMembersAsync(Guid projectId, Guid currentUserId, UserRole currentUserRole);
}