using System.Text.RegularExpressions;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.ValueObjects;

public sealed record Login
{
    public const int MaxLength = 100;
    public const int MinLength = 3;

    private static readonly Regex LoginRegex = new(
        @"^[a-z0-9._-]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private Login(string value)
    {
        Value = value;
    }

    public static Login Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Login cannot be empty.");

        var normalizedLogin = value.Trim().ToLowerInvariant();

        if (normalizedLogin.Length < MinLength)
        {
            throw new DomainException(
                $"Login cannot be shorter than {MinLength} characters.");
        }

        if (normalizedLogin.Length > MaxLength)
        {
            throw new DomainException(
                $"Login cannot be longer than {MaxLength} characters.");
        }

        if (!LoginRegex.IsMatch(normalizedLogin))
        {
            throw new DomainException(
                "Login can contain only Latin letters, numbers, dots, underscores and hyphens.");
        }

        return new Login(normalizedLogin);
    }

    public override string ToString() => Value;
}