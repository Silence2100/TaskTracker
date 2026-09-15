using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskTracker.Api.Authorization;
using TaskTracker.Api.Extensions;
using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IProjectService _projectService;
    private readonly IAuthorizationService _authorizationService;

    public TasksController(ITaskService taskService, IProjectService projectService, IAuthorizationService authorizationService)
    {
        _taskService = taskService;
        _projectService = projectService;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetAll()
    {
        var userId = User.GetUserId();

        if (userId is null)
            return Unauthorized();

        var tasks = await _taskService.GetByProjectMemberIdAsync(userId.Value);

        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);

        if (task is null)
            return NotFound();

        return Ok(task);
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

        if (!result.ProjectExists)
            return NotFound();

        if (!result.AuthorIsProjectMember)
            return Forbid();

        if (!result.AssigneeIsProjectMember)
            return BadRequest("Assigned user is not a member of the project.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Task!.Id },
            result.Task);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required.");

        if (!Enum.IsDefined(typeof(TaskItemStatus), dto.Status))
            return BadRequest("Invalid task status.");

        if (dto.AssignedUserId == Guid.Empty)
            return BadRequest("AssignedUserId is invalid.");

        var task = await _taskService.GetByIdAsync(id);

        if (task is null)
            return NotFound();

        var isUpdated = await _taskService.UpdateAsync(id, dto);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);

        if (task is null)
            return NotFound();

        var isDeleted = await _taskService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound();

        return NoContent();
    }
}