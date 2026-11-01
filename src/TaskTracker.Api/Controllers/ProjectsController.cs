using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Authorization;
using TaskTracker.Api.Extensions;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();

        return Ok(projects);
    }

    [HttpGet("{projectId:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid projectId)
    {
        var project = await _projectService.GetByIdAsync(projectId);

        if (project is null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto dto)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _projectService.CreateAsync(dto, userId.Value);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return CreatedAtAction(
            nameof(GetById),
            new { projectId = result.Value.Id },
            result.Value);
    }

    [HttpDelete("{projectId:guid}")]
    public async Task<IActionResult> Delete(Guid projectId)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _projectService.DeleteAsync(projectId, userId.Value);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return NoContent();
    }
}