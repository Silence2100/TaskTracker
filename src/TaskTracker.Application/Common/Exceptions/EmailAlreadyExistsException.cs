namespace TaskTracker.Application.Common.Exceptions;

public sealed class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException() : base("Email already exists.") { }
}