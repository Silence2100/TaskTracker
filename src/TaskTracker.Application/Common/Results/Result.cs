namespace TaskTracker.Application.Common.Results;

public sealed class Result<T>
{
    public ResultStatus Status { get; }
    public T? Value { get; }

    private Result(ResultStatus status, T? value = default)
    {
        Status = status;
        Value = value;
    }

    public static Result<T> Success(T value) 
        => new(ResultStatus.Success, value);

    public static Result<T> NotFound()
        => new(ResultStatus.NotFound);

    public static Result<T> Forbidden()
        => new(ResultStatus.Forbidden);
}