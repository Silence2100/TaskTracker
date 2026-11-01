namespace TaskTracker.Application.Common.Errors;

public static class ProjectErrors
{
    public static readonly Error NotFound = new(
        "Project.NotFound",
        "Project was not found.",
        ErrorType.NotFound);

    public static readonly Error AccessDenied = new(
        "Project.AccessDenied",
        "User is not allowed to manage this project.",
        ErrorType.Authorization);

    public static readonly Error InvalidName = new(
        "Project.InvalidName",
        "Project name must contain 1 to 200 characters.",
        ErrorType.Validation);
}