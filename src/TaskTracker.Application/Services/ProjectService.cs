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

    public async Task<MembersResult> GetMembers(Guid? userId, Guid projectId)
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

    public async Task<CreateProjectResult> CreateAsync(CreateProjectDto dto, Guid userId)
    {
        CreateProjectResult result = new();

        var canCreateProject = await _projectRepository.HasOwnerRoleAsync(userId);

        if (!canCreateProject)
        {
            result.CanCreateProject = false;

            return result;
        }

        var owner = await _userRepository.ReadByIdAsync(userId);

        if (owner is null)
        {
            result.CanCreateProject = false;

            return result;
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Members =
            [
                new ProjectMember
                {
                    UserId = userId,
                    Role = ProjectRole.Owner
                }
            ]
        };

        var createdProject = await _projectRepository.CreateAsync(project);

        result.CanCreateProject = true;
        result.Project = createdProject.ToDto();

        return result;
    }
}