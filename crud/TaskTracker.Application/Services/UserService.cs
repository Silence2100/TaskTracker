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

        return users.Select(user => user.ToDto()).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        return user.ToDto();
    }

    public async Task<UserDto?> RegisterAsync(RegisterUserDto dto)
    {
        var login = Login.Create(dto.Login);
        var email = Email.Create(dto.Email);

        var userWithSameLogin = await _userRepository.GetByLoginAsync(login);

        if (userWithSameLogin is not null)
            return null;

        if (await _userRepository.HasEmailAsync(email))
            return null;

        var passwordHash = _passwordHasher.Hash(dto.Password);

        var user = User.Register(login, email, passwordHash, dto.Name);

        await _userRepository.RegisterAsync(user);

        return user.ToDto();
    }
}