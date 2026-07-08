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

    public async Task<UserDto?> CreateAsync(CreateUserDto dto)
    {
        var email = Email.Create(dto.Email);
        var passwordHash = _passwordHasher.Hash(dto.Password);

        var user = User.Register(
            dto.Login,
            email,
            passwordHash,
            dto.Name);

        var userWithSameLogin = await _userRepository.GetByLoginAsync(user.Login);

        if (userWithSameLogin is not null)
            return null;

        var userWithSameEmail = await _userRepository.GetByEmailAsync(user.Email);

        if (userWithSameEmail is not null)
            return null;

        var createdUser = await _userRepository.CreateAsync(user);

        return _mapper.Map<UserDto>(createdUser);
    }
}