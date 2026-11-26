namespace TaskTracker.Application.Common.Exceptions;

public sealed class LoginAlreadyExistsException : Exception
{
    public LoginAlreadyExistsException() : base("Login already exists.") { }
}