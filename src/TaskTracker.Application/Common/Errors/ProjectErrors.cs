namespace TaskTracker.Application.Common.Errors;

public static class ProjectErrors
{
    public static readonly Error NotFound = new(
        "Project.NotFound",
        "Project was not found.",
        ErrorType.NotFound);
}