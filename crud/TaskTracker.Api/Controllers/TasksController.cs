using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Tasks;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Enums;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var tasks = await _taskService.GetAllAsync();

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
    public async Task<ActionResult<TaskDto>> Create(CreateTaskDto dto)
    {
        if (dto.ProjectId == Guid.Empty)
            return BadRequest("ProjectId is required.");

        if (dto.AuthorId == Guid.Empty)
            return BadRequest("AuthorId is required.");

        if (dto.AssignedUserId == Guid.Empty)
            return BadRequest("AssignedUserId is invalid.");

        if (string.IsNullOrWhiteSpace(dto.Title))
            return BadRequest("Title is required.");

        var createdTask = await _taskService.CreateAsync(dto);

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

        var isUpdated = await _taskService.UpdateAsync(id, dto);

        if (isUpdated == false)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var isDeleted = await _taskService.DeleteAsync(id);

        if (isDeleted == false)
            return NotFound();

        return NoContent();
    }
}