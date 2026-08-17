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

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var projects = await _projectService.GetByMemberIdAsync(userId.Value);

        return Ok(projects);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
            return NotFound();

        return Ok(project);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetAllMembers(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
            return NotFound();

        var members = await _projectService.GetMembersAndOwnerAsync(id);

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

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid ownerId, Guid projectId)
    {
        var result = await _projectService.GetAccessResult(ownerId, projectId);

        if (result is null)
            return NotFound();

        if (result.OkMessage is not null)
            return Ok(await _projectService.GetMembersAndOwnerAsync(projectId));

        return Forbid();
    }
}