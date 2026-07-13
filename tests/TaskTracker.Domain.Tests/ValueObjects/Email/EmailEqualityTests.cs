using TaskTracker.Domain.ValueObjects;

namespace TaskTracker.Domain.Tests.ValueObjects.EmailTests;

public sealed class EmailEqualityTests
{
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