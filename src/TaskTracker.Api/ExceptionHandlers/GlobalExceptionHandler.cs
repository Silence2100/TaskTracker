using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Common.Exceptions;
using TaskTracker.Domain.Common;

namespace TaskTracker.Api.ExceptionHandlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title) = exception switch
        {
            UserNotFoundException => (StatusCodes.Status404NotFound, "User not found"),

            LoginAlreadyExistsException => (StatusCodes.Status409Conflict, "Login already exists"),

            EmailAlreadyExistsException => (StatusCodes.Status409Conflict, "Email already exists"),

            InvalidLoginException => (StatusCodes.Status401Unauthorized, "Invalid login or password"),

            DomainException => (StatusCodes.Status400BadRequest, "Validation failed"),

            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "An exception occurred. TraceId: {TraceId}", context.TraceIdentifier);
        }
        else
        {
            logger.LogWarning("An exception occurred. Message: {ExceptionMessage}. TraceId: {TraceId}", exception.Message, context.TraceIdentifier);
        }

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception is DomainException
                    ? exception.Message
                    : null,
                Instance = context.Request.Path,
                Extensions =
                {
                    ["traceId"] = context.TraceIdentifier
                }
            },
            options: null,
            contentType: "application/problem+json",
            cancellationToken: cancellationToken);

        return true;
    }
}