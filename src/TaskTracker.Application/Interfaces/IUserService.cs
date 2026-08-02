using TaskTracker.Application.DTOs.Users;

namespace TaskTracker.Application.Interfaces;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> RegisterAsync(RegisterUserDto dto);
}