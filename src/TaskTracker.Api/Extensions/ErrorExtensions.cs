using Microsoft.AspNetCore.Mvc;
using TaskTracker.Application.Common;

namespace TaskTracker.Api.Extensions;

public static class ErrorExtensions
{
    public static ActionResult ToActionResult(this Error error, ControllerBase controller)
    {
        return error.Type switch
        {
            ErrorType.Validation =>
                controller.BadRequest(new
                {
                    error.Code,
                    error.Message
                }),

            ErrorType.NotFound =>
                controller.NotFound(new
                {
                    error.Code,
                    error.Message
                }),

            ErrorType.Authorization =>
                controller.Forbid(),

            ErrorType.Conflict =>
                controller.Conflict(new
                {
                    error.Code,
                    error.Message
                }),

            ErrorType.None =>
                throw new InvalidOperationException("Cannot convert Error.None to an ActionResult."),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(error.Type),
                    error.Type,
                    "Unknown error type.")
        };
    }
}