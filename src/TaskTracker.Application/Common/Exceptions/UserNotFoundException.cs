namespace TaskTracker.Application.Common.Exceptions;

public sealed class UserNotFoundException : Exception
{
    public UserNotFoundException(Guid userId) : base($"User with ID '{userId}' was not found.") { }
}