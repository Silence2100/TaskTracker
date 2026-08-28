using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Authorization;
using TaskTracker.Api.Extensions;
using TaskTracker.Application.Common;
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
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<List<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid projectId)
    {
        var project = await _projectService.GetByIdAsync(projectId);

        if (project is null)
            return NotFound();

        return Ok(project);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid projectId)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return NotFound();

        var result = await _projectService.GetMembers(userId, projectId);

        if (result.ProjectId is null)
            return NotFound();

        if (!result.CanGetMembers)
            return Unauthorized();

        return Ok(result.Members);
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
}