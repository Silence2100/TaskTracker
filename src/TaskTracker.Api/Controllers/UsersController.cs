using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Authorization;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userService.GetAllAsync();

        _logger.LogInformation("Retrieved {Count} users", users.Count);

        return users;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        _logger.LogInformation("Retrieved user with ID {UserId}", id);

        return Ok(user);
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegisterUserDto dto)
    {
        await _userService.RegisterAsync(dto);

        return Created();
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> LoginAsync(LoginUserDto dto)
    {
        var result = await _userService.LoginAsync(dto);

        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        await _userService.UpdateAsync(id, dto);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.Admin)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _userService.DeleteAsync(id);

        return Ok();
    }
}