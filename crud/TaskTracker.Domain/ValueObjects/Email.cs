using System.Text.RegularExpressions;
using TaskTracker.Domain.Common;

namespace TaskTracker.Domain.ValueObjects;

public sealed record Email
{
    private const int MaxLength = 255;

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.");

        var normalizedEmail = value.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > MaxLength)
            throw new DomainException($"Email cannot be longer than {MaxLength} characters.");

        if (!EmailRegex.IsMatch(normalizedEmail))
            throw new DomainException("Email has invalid format.");

        return new Email(normalizedEmail);
    }

    public override string ToString() => Value;
}