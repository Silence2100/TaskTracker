using AutoMapper;
using Moq;
using TaskTracker.Application.DTOs.Users;
using TaskTracker.Application.Interfaces;
using TaskTracker.Application.Services;
using TaskTracker.Domain.Entities;
using TaskTracker.Domain.ValueObjects;
using Xunit;

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
    public async Task RegisterAsync_WhenLoginAndEmailAreUnique_RegistersUserAndReturnsUserDto()
    {
        var dto = new RegisterUserDto
        {
            Login = " New.User ",
            Email = " USER@Example.COM ",
            Password = "strong-password",
            Name = " Egor "
        };

        const string passwordHash = "hashed-password";

        User? savedUser = null;

        _userRepositoryMock
            .Setup(repository => repository.GetByLoginAsync(
                It.Is<Login>(login => login.Value == "new.user")))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(
                It.Is<Email>(email => email.Value == "user@example.com")))
            .ReturnsAsync((User?)null);

        _passwordHasherMock
            .Setup(hasher => hasher.Hash(dto.Password))
            .Returns(passwordHash);

        _userRepositoryMock
            .Setup(repository => repository.RegisterAsync(It.IsAny<User>()))
            .Callback<User>(user => savedUser = user)
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(mapper => mapper.Map<UserDto>(It.IsAny<User>()))
            .Returns((User user) => new UserDto
            {
                Id = user.Id,
                Login = user.Login.Value,
                Email = user.Email.Value,
                Name = user.Name
            });

        UserDto? result = await _userService.RegisterAsync(dto);

        Assert.NotNull(result);

        User registeredUser = Assert.IsType<User>(savedUser);

        Assert.Equal("new.user", registeredUser.Login.Value);
        Assert.Equal("user@example.com", registeredUser.Email.Value);
        Assert.Equal(passwordHash, registeredUser.PasswordHash);
        Assert.Equal("Egor", registeredUser.Name);

        Assert.Equal(registeredUser.Id, result.Id);
        Assert.Equal("new.user", result.Login);
        Assert.Equal("user@example.com", result.Email);
        Assert.Equal("Egor", result.Name);

        _userRepositoryMock.Verify(
            repository => repository.GetByLoginAsync(
                It.Is<Login>(login => login.Value == "new.user")),
            Times.Once);

        _userRepositoryMock.Verify(
            repository => repository.GetByEmailAsync(
                It.Is<Email>(email => email.Value == "user@example.com")),
            Times.Once);

        _passwordHasherMock.Verify(
            hasher => hasher.Hash(dto.Password),
            Times.Once);

        _userRepositoryMock.Verify(
            repository => repository.RegisterAsync(
                It.Is<User>(user =>
                    user.Login.Value == "new.user" &&
                    user.Email.Value == "user@example.com" &&
                    user.PasswordHash == passwordHash &&
                    user.Name == "Egor")),
            Times.Once);

        _mapperMock.Verify(
            mapper => mapper.Map<UserDto>(
                It.Is<User>(user => ReferenceEquals(user, registeredUser))),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenLoginAlreadyExists_ReturnsNullAndStopsRegistration()
    {
        var dto = CreateRegisterUserDto();

        var existingUser = User.Register(
            Login.Create(dto.Login),
            Email.Create("another@example.com"),
            "existing-password-hash",
            "Existing User");

        _userRepositoryMock
            .Setup(repository => repository.GetByLoginAsync(
                It.Is<Login>(login => login.Value == "new.user")))
            .ReturnsAsync(existingUser);

        UserDto? result = await _userService.RegisterAsync(dto);

        Assert.Null(result);

        _userRepositoryMock.Verify(
            repository => repository.GetByLoginAsync(
                It.Is<Login>(login => login.Value == "new.user")),
            Times.Once);

        _userRepositoryMock.Verify(
            repository => repository.GetByEmailAsync(It.IsAny<Email>()),
            Times.Never);

        _passwordHasherMock.Verify(
            hasher => hasher.Hash(It.IsAny<string>()),
            Times.Never);

        _userRepositoryMock.Verify(
            repository => repository.RegisterAsync(It.IsAny<User>()),
            Times.Never);

        _mapperMock.Verify(
            mapper => mapper.Map<UserDto>(It.IsAny<User>()),
            Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsNullAndStopsRegistration()
    {
        var dto = CreateRegisterUserDto();

        var existingUser = User.Register(
            Login.Create("another.user"),
            Email.Create(dto.Email),
            "existing-password-hash",
            "Existing User");

        _userRepositoryMock
            .Setup(repository => repository.GetByLoginAsync(
                It.Is<Login>(login => login.Value == "new.user")))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(
                It.Is<Email>(email => email.Value == "user@example.com")))
            .ReturnsAsync(existingUser);

        UserDto? result = await _userService.RegisterAsync(dto);

        Assert.Null(result);

        _userRepositoryMock.Verify(
            repository => repository.GetByLoginAsync(
                It.Is<Login>(login => login.Value == "new.user")),
            Times.Once);

        _userRepositoryMock.Verify(
            repository => repository.GetByEmailAsync(
                It.Is<Email>(email => email.Value == "user@example.com")),
            Times.Once);

        _passwordHasherMock.Verify(
            hasher => hasher.Hash(It.IsAny<string>()),
            Times.Never);

        _userRepositoryMock.Verify(
            repository => repository.RegisterAsync(It.IsAny<User>()),
            Times.Never);

        _mapperMock.Verify(
            mapper => mapper.Map<UserDto>(It.IsAny<User>()),
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