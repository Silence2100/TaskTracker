using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappings;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public ProjectService(IProjectRepository projectRepository, IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }

    public async Task<List<ProjectDto>> GetAllAsync(Guid currentUserId, UserRole currentUserRole)
    {
        var projects = currentUserRole == UserRole.Admin
            ? await _projectRepository.GetAllAsync()
            : await _projectRepository.GetAllForUserAsync(currentUserId);

        return projects.Select(project => project.ToDto()).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, Guid currentUserId, UserRole currentUserRole)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project is null)
            return null;

        if (currentUserRole != UserRole.Admin)
        {
            var isMember = await _projectRepository.IsMemberAsync(id, currentUserId);

            if (!isMember)
                return null;
        }

        return project.ToDto();
    }

    public async Task<ProjectDto?> CreateAsync(CreateProjectDto dto, Guid ownerUserId)
    {
        var owner = await _userRepository.GetByIdAsync(ownerUserId);

        if (owner is null)
            return null;

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Members = new List<ProjectMember>
            {
                new()
                {
                    UserId = ownerUserId,
                    Role = ProjectRole.Owner
                }
            }
        };

        var createdProject = await _projectRepository.CreateAsync(project);

        return createdProject.ToDto();
    }

    public async Task<List<ProjectMemberDto>?> GetMembersAsync(Guid projectId, Guid currentUserId, UserRole currentUserRole)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return null;

        if (currentUserRole != UserRole.Admin)
        {
            var isMember = await _projectRepository.IsMemberAsync(projectId, currentUserId);

            if (!isMember)
                return null;
        }

        var members = await _projectRepository.GetMembersAsync(projectId);

        return members.Select(member => member.ToDto()).ToList();
    }
}