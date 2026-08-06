using Microsoft.AspNetCore.Authorization;

using TaskTracker.Api.Extensions;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Authorization.Projects;

public sealed class ProjectMemberHandler : AuthorizationHandler<ProjectMemberRequirement, Guid>
{
    private readonly IProjectRepository _projectRepository;

    public ProjectMemberHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectMemberRequirement requirement, Guid projectId)
    {
        var userId = context.User.GetUserId();

        if (userId is null)
            return;

        var isMember = await _projectRepository.IsMemberAsync(projectId, userId.Value);

        if (isMember)
            context.Succeed(requirement);
    }   
}