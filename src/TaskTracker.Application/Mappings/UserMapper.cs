using TaskTracker.Application.DTOs.Users;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Application.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Login = user.Login.Value,
            Email = user.Email.Value,
            Name = user.Name,
            Role = user.Role
        };
    }
}