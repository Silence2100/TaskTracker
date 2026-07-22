using AutoMapper;
using Moq;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Application.Tests.Services;

public sealed class UserServiceRegisterTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    private readonly UserService _userService;

    public UserServiceRegisterTests()
    {
        _userService = new UserService(_userRepositoryMock.Object, _passwordHasherMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenDataIsUnique_RegistersUserAndReturnsResult()
    {
        var dto = CreateRegisterUserDto();

        var expectedResult = new UserDto
        {
            Id = Guid.NewGuid(),
            Login = dto.Login,
            Email = dto.Email,
            Name = dto.Name
        };

        _userRepositoryMock
            .Setup(repository => repository.GetByLoginAsync(It.IsAny<Login>()))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(repository => repository.HasEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync(false);

        _passwordHasherMock
            .Setup(hasher => hasher.Hash(dto.Password))
            .Returns("hashed-password");

        _userRepositoryMock
            .Setup(repository => repository.RegisterAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(mapper => mapper.Map<UserDto>(It.IsAny<User>()))
            .Returns(expectedResult);

        UserDto? result = await _userService.RegisterAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.Login, result.Login);
        Assert.Equal(expectedResult.Email, result.Email);
        Assert.Equal(expectedResult.Name, result.Name);

        _userRepositoryMock.Verify(
            repository => repository.RegisterAsync(It.IsAny<User>()));
    }

    [Fact]
    public async Task RegisterAsync_WhenLoginAlreadyExists_ReturnsNull()
    {
        var dto = CreateRegisterUserDto();

        var existingUser = User.Register(
            Login.Create(dto.Login),
            Email.Create("another@example.com"),
            "existing-password-hash",
            "Existing User");

        _userRepositoryMock
            .Setup(repository => repository.GetByLoginAsync(It.IsAny<Login>()))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(repository => repository.HasEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync(false);

        UserDto? result = await _userService.RegisterAsync(dto);

        Assert.Null(result);

        _userRepositoryMock.Verify(
            repository => repository.RegisterAsync(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsNull()
    {
        var dto = CreateRegisterUserDto();

        _userRepositoryMock
            .Setup(repository => repository.GetByLoginAsync(It.IsAny<Login>()))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(repository => repository.HasEmailAsync(It.IsAny<Email>()))
            .ReturnsAsync(true);

        UserDto? result = await _userService.RegisterAsync(dto);

        Assert.Null(result);

        _userRepositoryMock.Verify(
            repository => repository.RegisterAsync(It.IsAny<User>()),
            Times.Never);
    }

    private static RegisterUserDto CreateRegisterUserDto()
    {
        return new RegisterUserDto
        {
            Login = "new.user",
            Email = "user@example.com",
            Password = "strong-password",
            Name = "Egor"
        };
    }
}