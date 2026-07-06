using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        var createdUser = await _userService.CreateAsync(dto);

        if (createdUser is null)
            return Conflict("User with same login or email already exists.");

        return CreatedAtAction(
            nameof(GetById), 
            new { id = createdUser.Id }, 
            createdUser);
    }
}