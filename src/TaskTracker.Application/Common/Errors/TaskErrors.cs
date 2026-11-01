namespace TaskTracker.Application.Common.Errors;

public static class TaskErrors
{
    public static readonly Error NotFound = new(
        "Task.NotFound",
        "Task was not found in the specified project.",
        ErrorType.NotFound);

    public static readonly Error AccessDenied = new(
        "Task.AccessDenied",
        "User is not a member of the project.",
        ErrorType.Authorization);

    public static readonly Error AuthorNotProjectMember = new(
        "Task.AuthorNotProjectMember",
        "Task author is not a member of the project.",
        ErrorType.Authorization);

    public static readonly Error AssigneeNotProjectMember = new(
        "Task.AssigneeNotProjectMember",
        "Assigned user is not a member of the project.",
        ErrorType.Validation);
}