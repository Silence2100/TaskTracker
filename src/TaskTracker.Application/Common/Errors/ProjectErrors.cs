namespace TaskTracker.Application.Common.Errors;

public static class ProjectErrors
{
    public static readonly Error NotFound = new(
        "Project.NotFound",
        "Project was not found.",
        ErrorType.NotFound);

    public static readonly Error CreationNotAllowed = new(
        "Project.CreationNotAllowed",
        "User is not allowed to create a project.",
        ErrorType.Authorization);

    public static readonly Error MembersAccessDenied = new(
        "Project.MembersAccessDenied",
        "User is not allowed to view project members.",
        ErrorType.Authorization);
}