using TaskTracker.Application.Common;
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

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();

        return projects
            .Select(project => project.ToDto())
            .ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        return project?.ToDto();
    }

    public async Task<MembersResult> GetMembers(Guid userId, Guid projectId)
    {
        MembersResult result = new();

        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
        {
            result.ProjectId = null;

            return result;
        }

        if (project.TryGetMembers(userId, out var members) == false)
        {
            result.CanGetMembers = false;

            return result;
        }

        result.Members = members.Select(member => member.ToDto()).ToList();

        return result;
    }

    public async Task<List<ProjectDto>> GetByMemberIdAsync(Guid memberId)
    {
        var projects = await _projectRepository.GetByMemberIdAsync(memberId);

        return projects
            .Select(project => project.ToDto())
            .ToList();
    }

    public async Task<ProjectDto?> CreateAsync(CreateProjectDto dto, Guid ownerUserId)
    {
        var owner = await _userRepository.ReadByIdAsync(ownerUserId);

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
}