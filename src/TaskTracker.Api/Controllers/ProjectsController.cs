using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Authorization;
using TaskTracker.Api.Extensions;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    public ProjectsController(IProjectService projectService, IAuthorizationService authorizationService    )
    {
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var projects = await _projectService.GetByMemberIdAsync(userId.Value);

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, id, Policies.ProjectMember);

        if (!authorizationResult.Succeeded)
            return Forbid();

        return Ok(project);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
            return NotFound();

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, id, Policies.ProjectMember);

        if (!authorizationResult.Succeeded)
            return Forbid();

        var members = await _projectService.GetMembersAsync(id);

        return Ok(members);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Project name is required.");

        var ownerUserId = User.GetUserId();

        if (ownerUserId is null)
            return Unauthorized();

        var createdProject = await _projectService.CreateAsync(dto, ownerUserId.Value);

        if (createdProject is null)
            return Unauthorized();

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id },
            createdProject);
    }
}