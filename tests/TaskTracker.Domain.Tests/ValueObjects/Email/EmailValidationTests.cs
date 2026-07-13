using TaskTracker.Domain.Common;
using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Domain.Tests.ValueObjects.EmailTests;

public sealed class EmailValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("EGOR.TYURIN")]
    [InlineData("@")]
    [InlineData("@mail.com")]
    [InlineData("EGOR.TYURIN@")]
    [InlineData("EGOR.TYURIN@mail")]
    [InlineData("EGOR.TYURIN@@mail.com")]
    [InlineData("EGOR@TYURIN@mail.com")]
    public void Create_ThrowsDomainException_WhenValueIsInvalid(string? invalidEmail)
    {
        // Act
        Action action = () => Email.Create(invalidEmail!);

        // Assert
        Assert.Throws<DomainException>(action);
    }
}