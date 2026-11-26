namespace TaskTracker.Application.Common.Exceptions;

public sealed class InvalidLoginException : Exception
{
    public InvalidLoginException() : base("Invalid login or password.") { }
}