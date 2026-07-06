using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(MapToDto)
            .ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        return MapToDto(user);
    }

    public async Task<UserDto?> CreateAsync(CreateUserDto dto)
    {
        var login = dto.Login.Trim();
        var email = dto.Email.Trim().ToLower();
        var name = dto.Name.Trim();

        var userWithSameLogin = await _userRepository.GetByLoginAsync(login);

        if (userWithSameLogin is not null)
            return null;

        var userWithSameEmail = await _userRepository.GetByEmailAsync(email);

        if (userWithSameEmail is not null)
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            Email = email,
            Password = dto.Password,
            Name = name
        };

        var createdUser = await _userRepository.CreateAsync(user);

        return MapToDto(createdUser);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Email = user.Email,
            Name = user.Name
        };
    }
}