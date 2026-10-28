using TaskTracker.Application.Common;
using TaskTracker.Application.Common.Errors;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappings;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IUserRepository _userRepository;

    public ProjectService(
        IProjectRepository projectRepository,
        IMemberRepository memberRepository,
        IUserRepository userRepository)
    {
        _projectRepository = projectRepository;
        _memberRepository = memberRepository;
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

    public async Task<Result<List<ProjectMemberDto>>> GetMembers(Guid userId, Guid projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return Result<List<ProjectMemberDto>>.Failure(ProjectErrors.NotFound);

        var role = await _memberRepository.GetRoleAsync(projectId, userId);

        if (role != ProjectRole.Owner)
            return Result<List<ProjectMemberDto>>.Failure(ProjectErrors.MembersAccessDenied);

        var members = await _memberRepository.GetByProjectIdAsync(projectId);

        var memberDtos = members
            .Select(member => member.ToDto())
            .ToList();

        return Result<List<ProjectMemberDto>>.Success(memberDtos);
    }

    public async Task<List<ProjectDto>> GetByMemberIdAsync(Guid memberId)
    {
        var projects = await _projectRepository.GetByMemberIdAsync(memberId);

        return projects
            .Select(project => project.ToDto())
            .ToList();
    }

    public async Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto, Guid userId)
    {
        var canCreateProject = await _memberRepository.HasOwnerRoleAsync(userId);

        if (!canCreateProject)
            return Result<ProjectDto>.Failure(ProjectErrors.CreationNotAllowed);

        var owner = await _userRepository.ReadByIdAsync(userId);

        if (owner is null)
            return Result<ProjectDto>.Failure(ProjectErrors.CreationNotAllowed);

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

        return Result<ProjectDto>.Success(createdProject.ToDto());
    }
}