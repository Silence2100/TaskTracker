using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Extensions;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:guid}/members")]
public class ProjectMembersController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectMembersController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid projectId)
    {
        var currentUserId = User.GetUserId();

        if (currentUserId is null)
            return Unauthorized();

        var result = await _projectService.GetMembers(currentUserId.Value, projectId);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectMemberDto>> AddMember(Guid projectId, [FromQuery] Guid userId)
    {
        var currentUserId = User.GetUserId();

        if (currentUserId is null)
            return Unauthorized();

        var result = await _projectService.AddMemberAsync(projectId, userId, currentUserId.Value);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveMember(Guid projectId, [FromQuery] Guid userId)
    {
        var currentUserId = User.GetUserId();

        if (currentUserId is null)
            return Unauthorized();

        var result = await _projectService.RemoveMemberAsync(projectId, userId, currentUserId.Value);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult<ProjectMemberDto>> UpdateMemberRole(Guid projectId, [FromBody] UpdateProjectMemberRoleDto dto)
    {
        var currentUserId = User.GetUserId();

        if (currentUserId is null)
            return Unauthorized();

        var result = await _projectService.UpdateMemberRoleAsync(projectId, dto, currentUserId.Value);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return Ok(result.Value);
    }
}