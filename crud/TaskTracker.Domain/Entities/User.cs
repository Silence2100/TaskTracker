using TaskTracker.Domain.Common;
using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Domain.Entities;

public class User
{
    private const int LoginMaxLength = 100;
    private const int NameMaxLength = 150;
    private const int PasswordMaxLength = 500;

    public Guid Id { get; private set; }
    public string Login { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public List<ProjectMember> ProjectMembers { get; private set; } = new();
    public List<TaskItem> AuthoredTasks { get; private set; } = new();
    public List<TaskItem> AssignedTasks { get; private set; } = new();

    private User() { } // For EF Core

    private User(Guid id, string login, Email email, string passwordHash, string name)
    {
        Id = id;
        Login = login;
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
    }

    public static User Register(string login, Email email, string passwordHash, string name)
    {
        var normalizedLogin = NormalizeRequiredString(
            login,
            LoginMaxLength,
            "Login cannot be empty.",
            $"Login cannot be longer than {LoginMaxLength} characters.");

        var normalizedName = NormalizeRequiredString(
            name,
            NameMaxLength,
            "Name cannot be empty.",
            $"Name cannot be longer than {NameMaxLength} characters.");

        var normalizedPasswordHash = NormalizeRequiredString(
            passwordHash,
            PasswordMaxLength,
            "Password hash cannot be empty.",
            $"Password hash cannot be longer than {PasswordMaxLength} characters.");

        return new User(Guid.NewGuid(), normalizedLogin, email, normalizedPasswordHash, normalizedName);
    }

    private static string NormalizeRequiredString(string value, int maxLength, string emptyErrorMessage, string maxLengthErrorMessage)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(emptyErrorMessage);

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maxLength)
            throw new DomainException(maxLengthErrorMessage);

        return normalizedValue;
    }
}