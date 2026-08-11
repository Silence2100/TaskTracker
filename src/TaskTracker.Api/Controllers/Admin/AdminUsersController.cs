using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskTracker.Api.Authorization;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;

namespace TaskTracker.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = Policies.AdminPanel)]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;
    public AdminUsersController(IUserService userService)
    {
        _userService = userService;
    }

    /*[HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }*/

    /*[HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }*/
}