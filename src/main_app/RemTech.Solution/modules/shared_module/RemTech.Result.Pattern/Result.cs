namespace RemTech.Result.Pattern;

public class Result
{
    private readonly Error? _error;
    public bool IsSuccess { get; }
    public bool IsFailure { get; }

    public Error Error
    {
        get
        {
            if (IsSuccess)
                throw new ApplicationException("Attempt to access error of success result.");

            if (_error == null)
                throw new ApplicationException("Failed result does not contain error.");

            return _error;
        }
    }

    public Result()
    {
        IsSuccess = true;
        IsFailure = false;
        _error = null;
    }

    public Result(Error error)
    {
        _error = error;
        IsSuccess = false;
        IsFailure = true;
    }

    public Result(ValidationError error)
        : this((Error)error) { }

    public Result(Result result)
    {
        _error = result._error;
        IsSuccess = result.IsSuccess;
        IsFailure = result.IsFailure;
    }

    public static Result Success() => new();

    public static Result Failure(Error error) => new(error);

    public static Result Failures(params Result[] statuses)
    {
        Result? firstBad = statuses.FirstOrDefault(f => f.IsFailure);
        return firstBad == null
            ? throw new ApplicationException("Failure result was not found")
            : new Result(firstBad);
    }

    public static implicit operator Error(Result result) => result.Error;

    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<T> : Result
{
    private readonly T _value;

    public T Value
    {
        get
        {
            if (IsFailure)
                throw new ApplicationException("Attempt to access result of failure.");

            if (_value == null)
                throw new ApplicationException("Success result does not contain value.");

            return _value;
        }
    }

    public Result(T value)
    {
        _value = value;
    }

    public Result(Error error)
        : base(error)
    {
        _value = default!;
    }

    public Result(Result<T> result)
        : base(result.Error)
    {
        _value = result._value;
    }

    public Result(ValidationError error)
        : this((Error)error) { }

    public static Result<T> Failures(params Result<T>[] statuses)
    {
        Result<T>? firstBad = statuses.FirstOrDefault(f => f.IsFailure);
        return firstBad == null
            ? throw new ApplicationException("Failure result was not found")
            : new Result<T>(firstBad);
    }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(Error error) => new(error);

    public static implicit operator T(Result<T> result) => result.Value;

    public static implicit operator Error(Result<T> result) => result.Error;

    public static implicit operator Result<T>(Error error) => Failure(error);

    public static implicit operator Result<T>(T value) => Success(value);
}
