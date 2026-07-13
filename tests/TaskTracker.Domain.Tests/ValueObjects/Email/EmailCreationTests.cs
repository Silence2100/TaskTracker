using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Domain.Tests.ValueObjects.EmailTests;

public sealed class EmailCreationTests
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
}