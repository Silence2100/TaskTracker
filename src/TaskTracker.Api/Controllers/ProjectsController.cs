using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();

        if (userId is null || role is null)
            return Unauthorized();

        var projects = await _projectService.GetAllAsync(userId.Value, role.Value);

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();

        if (userId is null || role is null)
            return Unauthorized();

        var project = await _projectService.GetByIdAsync(id, userId.Value, role.Value);

        if (project is null)
            return NotFound();

        return Ok(project);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid id)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();

        if (userId is null || role is null)
            return Unauthorized();

        var members = await _projectService.GetMembersAsync(id, userId.Value, role.Value);

        if (members is null)
            return NotFound();

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