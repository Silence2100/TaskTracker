using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetAll()
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();

        if (userId is null || role is null)
            return Unauthorized();

        var tasks = await _taskService.GetAllAsync(userId.Value, role.Value);

        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDto>> GetById(Guid id)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();

        if (userId is null || role is null)
            return Unauthorized();

        var task = await _taskService.GetByIdAsync(id, userId.Value, role.Value);

        if (task is null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(CreateTaskDto dto)
    {
        if (dto.ProjectId == Guid.Empty)
            return BadRequest("ProjectId is required.");

        if (dto.AssignedUserId == Guid.Empty)
            return BadRequest("AssignedUserId is invalid.");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required.");

        var userId = User.GetUserId();
        var role = User.GetUserRole();

        if (userId is null || role is null)
            return Unauthorized();

        var createdTask = await _taskService.CreateAsync(dto, userId.Value, role.Value);

        if (createdTask is null)
            return NotFound();

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdTask.Id },
            createdTask);
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

        var userId = User.GetUserId();
        var role = User.GetUserRole();

        if (userId is null || role is null)
            return Unauthorized();

        var isUpdated = await _taskService.UpdateAsync(id, dto, userId.Value, role.Value);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        var role = User.GetUserRole();

        if (userId is null || role is null)
            return Unauthorized();

        var isDeleted = await _taskService.DeleteAsync(id, userId.Value, role.Value);

        if (!isDeleted)
            return NotFound();

        return NoContent();
    }
}