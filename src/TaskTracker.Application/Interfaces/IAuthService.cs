using TaskTracker.Application.DTOs.Auth;

namespace TaskTracker.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginUserDto dto);
}