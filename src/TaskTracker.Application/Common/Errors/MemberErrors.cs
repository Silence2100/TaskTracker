namespace TaskTracker.Application.Common.Errors;

public static class MemberErrors
{
    public static readonly Error NotFound = new(
        "Member.NotFound",
        "User is not a member of this project.",
        ErrorType.NotFound);

    public static readonly Error UserNotFound = new(
        "Member.UserNotFound",
        "User was not found.",
        ErrorType.NotFound);

    public static readonly Error AlreadyExists = new(
        "Member.AlreadyExists",
        "User is already a member of this project.",
        ErrorType.Conflict);

    public static readonly Error InvalidUserId = new(
        "Member.InvalidUserId",
        "User ID cannot be empty.",
        ErrorType.Validation);

    public static readonly Error InvalidRole = new(
        "Member.InvalidRole",
        "Invalid project member role.",
        ErrorType.Validation);

    public static readonly Error LastOwner = new(
        "Member.LastOwner",
        "Cannot remove or demote the last project owner.",
        ErrorType.Conflict);
}