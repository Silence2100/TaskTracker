using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Mappings;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(user => user.ToDto())
            .ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        return user
            .ToDto();
    }

    public async Task<UserDto?> RegisterAsync(RegisterUserDto dto)
    {
        var login = Login.Create(dto.Login);
        var email = Email.Create(dto.Email);

        if (await _userRepository.HasLoginAsync(login) || await _userRepository.HasEmailAsync(email))
            return null;

        var passwordHash = _passwordHasher.Hash(dto.Password);

        var user = User.Register(login, email, passwordHash, dto.Name);

        await _userRepository.RegisterAsync(user);

        return user.ToDto();
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        if (dto.Login is not null)
            user.UpdateLogin(Login.Create(dto.Login));

        if (dto.Email is not null)
            user.UpdateEmail(Email.Create(dto.Email));

        if (dto.Name is not null)
            user.UpdateName(dto.Name);

        if (dto.Role is not null)
            user.UpdateRole(dto.Role.Value);

        await _userRepository.UpdateAsync(user);

        return user.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }
}