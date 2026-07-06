using AutoMapper;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ProjectService(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<ProjectDto>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();

        return _mapper.Map<List<ProjectDto>>(projects);
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project is null)
            return null;

        return _mapper.Map<ProjectDto>(project);
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

        return _mapper.Map<ProjectDto>(createdProject);
    }

    public async Task<List<ProjectMemberDto>?> GetMembersAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return null;

        var members = await _projectRepository.GetMembersAsync(projectId);

        return _mapper.Map<List<ProjectMemberDto>>(members);
    }
}