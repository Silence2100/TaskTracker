using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Projects;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var projects = await _projectService.GetAllAsync();

        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project is null)
            return NotFound();

        return Ok(project);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<List<ProjectMemberDto>>> GetMembers(Guid id)
    {
        var members = await _projectService.GetMembersAsync(id);

        if (members is null)
            return NotFound();

        return Ok(members);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Project name is required.");

        if (dto.OwnerUserId == Guid.Empty)
            return BadRequest("OwnerUserId is required.");

        var createdProject = await _projectService.CreateAsync(dto);

        if (createdProject is null)
            return BadRequest("Owner user was not found.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProject.Id },
            createdProject);
    }
}