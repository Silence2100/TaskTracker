using Microsoft.AspNetCore.Mvc;

using TaskTracker.Application.Interfaces;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Application.DTOs.Users;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public UsersController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAllAsync()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync(RegisterUserDto dto)
    {
        var user = await _userService.RegisterAsync(dto);

        if (user is null)
            return Conflict();

        return Created();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync(LoginUserDto dto)
    {
        var user = await _authService.LoginAsync(dto);

        if (user is null)
            return Unauthorized();

        return Ok();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);

        if (user is null)
            return NotFound();

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var user = await _userService.DeleteAsync(id);

        if (user == false)
            return NotFound();

        return Ok();
    }
}