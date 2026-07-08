using Microsoft.AspNetCore.Identity;
using TaskTracker.Application.Interfaces;
using TaskTracker.Domain.Common;

namespace TaskTracker.Infrastructure.Security;

public class UserPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Password cannot be empty.");

        return _passwordHasher.HashPassword(new object(), password);
    }
}