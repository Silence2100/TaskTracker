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

    public ProjectService(IProjectRepository projectRepository, IMemberRepository memberRepository, IUserRepository userRepository)
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

    public async Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || dto.Name.Trim().Length > 200)
            return Result<ProjectDto>.Failure(ProjectErrors.InvalidName);

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            return Result<ProjectDto>.Failure(ProjectErrors.AccessDenied);

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

        var created = await _projectRepository.CreateAsync(project);

        return Result<ProjectDto>.Success(created.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid projectId,Guid userId)
    {
        var projectResult = await GetOwnedProjectAsync(projectId, userId);

        if (projectResult.IsFailure)
            return Result<bool>.Failure(projectResult.Error);

        var deleted = await _projectRepository.DeleteAsync(projectId);

        if (!deleted)
            return Result<bool>.Failure(ProjectErrors.NotFound);

        return Result<bool>.Success(true);
    }

    public async Task<Result<List<ProjectMemberDto>>> GetMembers(Guid userId, Guid projectId)
    {
        var projectResult = await GetOwnedProjectAsync(projectId, userId);

        if (projectResult.IsFailure)
            return Result<List<ProjectMemberDto>>.Failure(projectResult.Error);

        var members = await _memberRepository.GetByProjectIdAsync(projectId);

        var result = new List<ProjectMemberDto>();

        foreach (var member in members)
            result.Add(await ToMemberDtoAsync(member));

        return Result<List<ProjectMemberDto>>.Success(result);
    }

    public async Task<Result<ProjectMemberDto>> AddMemberAsync(Guid projectId, Guid userId, Guid currentUserId)
    {
        if (userId == Guid.Empty)
            return Result<ProjectMemberDto>.Failure(MemberErrors.InvalidUserId);

        var projectResult = await GetOwnedProjectAsync(projectId, currentUserId);

        if (projectResult.IsFailure)
            return Result<ProjectMemberDto>.Failure(projectResult.Error);

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            return Result<ProjectMemberDto>.Failure(MemberErrors.UserNotFound);

        var existingMember = await _memberRepository.GetAsync(projectId, userId);

        if (existingMember is not null)
            return Result<ProjectMemberDto>.Failure(MemberErrors.AlreadyExists);

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId,
            Role = ProjectRole.Member
        };

        await _memberRepository.AddAsync(member);

        return Result<ProjectMemberDto>.Success(await ToMemberDtoAsync(member));
    }

    public async Task<Result<bool>> RemoveMemberAsync(Guid projectId, Guid userId, Guid currentUserId)
    {
        if (userId == Guid.Empty)
            return Result<bool>.Failure(MemberErrors.InvalidUserId);

        var projectResult = await GetOwnedProjectAsync(projectId, currentUserId, trackTasks: true);

        if (projectResult.IsFailure)
            return Result<bool>.Failure(projectResult.Error);

        var project = projectResult.Value;

        var member = await _memberRepository.GetAsync(projectId, userId);

        if (member is null)
            return Result<bool>.Failure(MemberErrors.NotFound);

        if (member.Role == ProjectRole.Owner)
        {
            var members = await _memberRepository.GetByProjectIdAsync(projectId);

            var ownersCount = members.Count(item => item.Role == ProjectRole.Owner);

            if (ownersCount <= 1)
                return Result<bool>.Failure(MemberErrors.LastOwner);
        }

        project.UnassignTasksFromUser(userId);

        await _memberRepository.DeleteAsync(member);

        return Result<bool>.Success(true);
    }

    public async Task<Result<ProjectMemberDto>> UpdateMemberRoleAsync(Guid projectId, UpdateProjectMemberRoleDto dto, Guid currentUserId)
    {
        if (dto.UserId == Guid.Empty)
            return Result<ProjectMemberDto>.Failure(MemberErrors.InvalidUserId);

        if (!Enum.IsDefined(typeof(ProjectRole), dto.Role))
            return Result<ProjectMemberDto>.Failure(MemberErrors.InvalidRole);

        var projectResult = await GetOwnedProjectAsync(projectId, currentUserId);

        if (projectResult.IsFailure)
            return Result<ProjectMemberDto>.Failure(projectResult.Error);

        var member = await _memberRepository.GetAsync(projectId, dto.UserId);

        if (member is null)
            return Result<ProjectMemberDto>.Failure(MemberErrors.NotFound);

        if (member.Role == ProjectRole.Owner && dto.Role != ProjectRole.Owner)
        {
            var members = await _memberRepository.GetByProjectIdAsync(projectId);

            var ownersCount = members.Count(item => item.Role == ProjectRole.Owner);

            if (ownersCount <= 1)
                return Result<ProjectMemberDto>.Failure(MemberErrors.LastOwner);
        }

        member.Role = dto.Role;

        await _memberRepository.UpdateAsync(member);

        return Result<ProjectMemberDto>.Success(await ToMemberDtoAsync(member));
    }

    private async Task<Result<Project>> GetOwnedProjectAsync(Guid projectId, Guid currentUserId, bool trackTasks = false)
    {
        var project = trackTasks
            ? await _projectRepository.GetForUpdateAsync(projectId)
            : await _projectRepository.GetByIdAsync(projectId);

        if (project is null)
            return Result<Project>.Failure(ProjectErrors.NotFound);

        var role = await _memberRepository.GetRoleAsync(projectId, currentUserId);

        if (role != ProjectRole.Owner)
            return Result<Project>.Failure(ProjectErrors.AccessDenied);

        return Result<Project>.Success(project);
    }

    private async Task<ProjectMemberDto> ToMemberDtoAsync(ProjectMember member)
    {
        var user = await _userRepository.GetByIdAsync(member.UserId);

        return new ProjectMemberDto
        {
            UserId = member.UserId,
            UserName = user?.Name ?? string.Empty,
            UserEmail = user?.Email.Value ?? string.Empty,
            Role = member.Role
        };
    }
}