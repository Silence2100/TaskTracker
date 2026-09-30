namespace TaskTracker.Application.Common;

public sealed class Result<T>
{
    private readonly T? _value;

    private Result(T value)
    {
        _value = value;
        Error = Error.None;
    }

    private Result(Error error)
    {
        _value = default;
        Error = error;
    }

    public bool IsSuccess => Error == Error.None;

    public bool IsFailure => !IsSuccess;

    public T Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot access the value of a failed result.");
            }

            return _value!;
        }
    }

    public Error Error { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Failure(Error error)
    {
        if (error == Error.None)
        {
            throw new ArgumentException("A failure result must contain an error.", nameof(error));
        }

        return new Result<T>(error);
    }
}