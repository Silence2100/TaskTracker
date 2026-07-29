using AutoMapper;
using TaskTracker.Application.DTOs.Auth;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Application.Services;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public AuthService(IPasswordHasher passwordHasher, IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, IMapper mapper)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
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
            User = _mapper.Map<UserDto>(user)
        };
    }
}