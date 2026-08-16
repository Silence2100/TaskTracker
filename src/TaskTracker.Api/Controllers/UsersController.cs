using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskTracker.Api.Authorization;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Application.DTOs.Users;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userService.GetAllAsync();

        return users;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync(RegisterUserDto dto)
    {
        var user = await _userService.RegisterAsync(dto);

        if (user is null)
            return Conflict();

        return Created();
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync(LoginUserDto dto)
    {
        var result = await _userService.LoginAsync(dto);

        if (result is null)
            return Unauthorized();

        return Ok(result);
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);

        if (user is null)
            return NotFound();

        return Ok();
    }

    [Authorize(Policy = Policies.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var user = await _userService.DeleteAsync(id);

        if (user == false)
            return NotFound();

        return Ok();
    }
}