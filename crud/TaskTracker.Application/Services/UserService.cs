using AutoMapper;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
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

        return _mapper.Map<UserDto>(createdUser);
    }
}