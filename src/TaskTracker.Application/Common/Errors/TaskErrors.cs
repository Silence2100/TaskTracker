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

    public static readonly Error InvalidTitle = new(
        "Task.InvalidTitle",
        "Title must contain 1 to 200 characters.",
        ErrorType.Validation);

    public static readonly Error InvalidDescription = new(
        "Task.InvalidDescription",
        "Description cannot exceed 2000 characters.",
        ErrorType.Validation);

    public static readonly Error InvalidAssignee = new(
        "Task.InvalidAssignee",
        "Assigned user ID cannot be empty.",
        ErrorType.Validation);

    public static readonly Error InvalidStatus = new(
        "Task.InvalidStatus",
        "Invalid task status.",
        ErrorType.Validation);
}