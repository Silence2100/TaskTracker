using TaskTracker.Domain.Common;
using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Domain.Tests.ValueObjects.EmailTests;

public sealed class EmailTests
{
    [Fact]
    public void Create_ReturnsEmail_WhenValueIsValid()
    {
        // Arrange
        const string validEmail = "egor@gmail.com";

        // Act
        Email email = Email.Create(validEmail);

        // Assert
        Assert.NotNull(email);
        Assert.Equal(validEmail, email.Value);
    }

    [Fact]
    public void Create_StoresEmailInLowerCase_WhenValueContainsUpperCaseLetters()
    {
        // Arrange
        const string emailValue = "EGOR.TYURIN@MAIL.COM";

        // Act
        Email email = Email.Create(emailValue);

        // Assert
        Assert.Equal("egor.tyurin@mail.com", email.Value);
    }

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

    [Fact]
    public void Equals_ReturnsTrue_WhenEmailsDifferOnlyByLetterCase()
    {
        // Arrange
        Email email1 = Email.Create("egor@gmail.com");
        Email email2 = Email.Create("EGOR@gmail.com");

        // Act
        bool result = email1.Equals(email2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EqualityOperator_ReturnsTrue_WhenEmailsHaveSameValue()
    {
        // Arrange
        Email email1 = Email.Create("egor@gmail.com");
        Email email2 = Email.Create("EGOR@gmail.com");

        // Act
        bool result = email1 == email2;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenEmailsHaveDifferentValues()
    {
        // Arrange
        Email email1 = Email.Create("first@gmail.com");
        Email email2 = Email.Create("second@gmail.com");

        // Act
        bool result = email1.Equals(email2);

        // Assert
        Assert.False(result);
    }
}