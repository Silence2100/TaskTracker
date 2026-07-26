using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Common;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginUserDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);

            if (result is null)
                return Unauthorized("Invalid login or password.");

            return Ok(result);
        }
        catch (DomainException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}