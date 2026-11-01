using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Extensions;
using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:guid}/tasks")]
public class ProjectsTasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public ProjectsTasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetAll(Guid projectId)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _taskService.GetByProjectAsync(projectId, userId.Value);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpGet("{taskId:int}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid projectId, int taskId)
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _taskService.GetByIdAsync(projectId, taskId, userId.Value);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(Guid projectId, CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required.");

        if (dto.AssignedUserId == Guid.Empty)
            return BadRequest("AssignedUserId is invalid.");

        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var result = await _taskService.CreateAsync(projectId, dto, userId.Value);

        if (result.IsFailure)
            return result.Error.ToActionResult(this);

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                projectId,
                taskId = result.Value.Id
            },
            result.Value);
    }
}