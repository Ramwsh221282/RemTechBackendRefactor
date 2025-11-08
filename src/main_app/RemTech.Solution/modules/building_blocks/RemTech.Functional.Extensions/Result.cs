namespace RemTech.Functional.Extensions;

public class Result
{
    public Error Error { get; } = Error.NoError();
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    internal Result(Error error)
    {
        Error = error;
        IsSuccess = false;
    }

    internal Result()
    {
        IsSuccess = true;
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Failure<T>(Error error)
    {
        return new Result<T>(error);
    }

    public static Result Failure(Error error)
    {
        return new Result(error);
    }

    public static implicit operator Result(Error error)
    {
        return Failure(error);
    }
}

public class Result<T> : Result
{
    internal Result(T value)
    {
        Value = value;
    }

    internal Result(Error error) : base(error)
    {
        Value = default;
    }

    public T Value =>
        !IsSuccess
            ? throw new InvalidOperationException($"Нельзя получить доступ к неуспешному {nameof(Result)}")
            : field!;

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Error error)
    {
        return new Result<T>(error);
    }

    public static implicit operator T(Result<T> result)
    {
        return result.Value;
    }
}