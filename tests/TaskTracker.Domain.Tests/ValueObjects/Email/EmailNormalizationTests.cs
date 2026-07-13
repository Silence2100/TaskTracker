using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Domain.Tests.ValueObjects.EmailTests;

public sealed class EmailNormalizationTests
{
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
}