using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Interfaces;
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
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project is null)
            return null;

        return MapToDto(project);
    }

    public async Task<ProjectDto?> CreateAsync(CreateProjectDto dto)
    {
        var owner = await _userRepository.GetByIdAsync(dto.OwnerUserId);

        if (owner is null)
            return null;

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Members = new List<ProjectMember>
            {
                new ProjectMember
                {
                    UserId = dto.OwnerUserId,
                    Role = ProjectRole.Owner
                }
            }
        };

        var createdProject = await _projectRepository.CreateAsync(project);

        return MapToDto(createdProject);
    }

    public async Task<List<ProjectMemberDto>?> GetMembersAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return null;

        var members = await _projectRepository.GetMembersAsync(projectId);

        return members
            .Select(member => new ProjectMemberDto
            {
                UserId = member.UserId,
                UserName = member.User.Name,
                UserEmail = member.User.Email,
                Role = member.Role
            })
            .ToList();
    }

    private static ProjectDto MapToDto(Project project)
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