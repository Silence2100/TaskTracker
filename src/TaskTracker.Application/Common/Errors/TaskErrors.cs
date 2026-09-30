namespace TaskTracker.Application.Common.Errors;

public static class TaskErrors
{
    public static readonly Error AuthorNotProjectMember = new(
        "Task.AuthorNotProjectMember",
        "Task author is not a member of the project.",
        ErrorType.Authorization);

    public static readonly Error AssigneeNotProjectMember = new(
        "Task.AssigneeNotProjectMember",
        "Assigned user is not a member of the project.",
        ErrorType.Validation);
}