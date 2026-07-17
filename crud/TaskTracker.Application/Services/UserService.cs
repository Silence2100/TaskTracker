using AutoMapper;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return _mapper.Map<List<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
            return null;

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> RegisterAsync(RegisterUserDto dto)
    {
        var login = Login.Create(dto.Login);
        var email = Email.Create(dto.Email);

        var userWithSameLogin =
            await _userRepository.GetByLoginAsync(login);

        if (userWithSameLogin is not null)
            return null;

        var userWithSameEmail = await _userRepository.GetByEmailAsync(email);

        if (userWithSameEmail is not null)
            return null;

        var passwordHash = _passwordHasher.Hash(dto.Password);

        var user = User.Register(login, email, passwordHash, dto.Name);

        await _userRepository.RegisterAsync(user);

        return _mapper.Map<UserDto>(user);
    }
}