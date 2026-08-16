using TaskTracker.Application.DTOs.Auth;
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
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
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
        var user = await _userRepository.ReadByIdAsync(id);

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

    public async Task<AuthResponseDto?> LoginAsync(LoginUserDto dto)
    {
        var login = Login.Create(dto.Login);

        var user = await _userRepository.GetByLoginAsync(login);

        if (user is null)
            return null;

        if (!_passwordHasher.Verify(user.PasswordHash, dto.Password))
            return null;

        var accessToken = _jwtTokenGenerator.Generate(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            User = user.ToDto()
        };
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        if (dto.Name is not null)
            user.UpdateName(dto.Name);

        if (dto.Role is not null)
            user.UpdateRole(dto.Role.Value);

        if (dto.IsBlock is true)
            user.Block(dto.IsBlock);

        await _userRepository.UpdateAsync(user);

        return user.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }
}