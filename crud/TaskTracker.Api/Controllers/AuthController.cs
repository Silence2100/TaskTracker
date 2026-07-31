using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using TaskTracker.Application.Common;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Domain.Enums;
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

    [AllowAnonymous]
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

    [Authorize]
    [HttpGet("me")]
    public ActionResult<CurrentUserDto> GetCurrentUser()
    {
        var userIdValue = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        var login = User.Identity?.Name;

        var email = User.FindFirstValue(JwtRegisteredClaimNames.Email);

        var roleValue = User.FindFirstValue(JwtClaimNames.Role);

        if (!Guid.TryParse(userIdValue, out var userId) 
            || string.IsNullOrWhiteSpace(login) 
            || string.IsNullOrWhiteSpace(email) 
            || !Enum.TryParse<UserRole>(roleValue, ignoreCase: true, out var role))
        {
            return Unauthorized();
        }

        return Ok(new CurrentUserDto
        {
            Id = userId,
            Login = login,
            Email = email
        });
    }
}