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

    public async Task<Result<U>> ContinueAsync<U>(Func<T, Task<Result<U>>> continuation)
    {
        return IsFailure ? Failure<U>(Error) : await continuation(Value);
    }
    
    public Result<U> Continue<U>(Func<T, Result<U>> continuation)
    {
        return IsFailure ? Failure<U>(Error) : continuation(Value);
    }
    
    public Result<U> Continue<U>(Func<Result<U>> continuation)
    {
        return IsFailure ? Failure<U>(Error) : continuation();
    }
    
    public Result<U> Continue<U>(Func<U> continuation)
    {
        return IsFailure ? Failure<U>(Error) : Success(continuation());
    }
    
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

public static class ResultModule
{
    extension<T>(Result<T> result)
    {
        public Result<U> Continue<U>(Func<T, Result<U>> continuation)
        {
            return result.IsFailure ? Result.Failure<U>(result.Error) : continuation(result.Value);
        }
        
        public Result<U> Continue<U>(Func<Result<U>> continuation)
        {
            return result.IsFailure ? Result.Failure<U>(result.Error) : continuation();
        }
        
        public Result<U> Continue<U>(Result<U> continuation)
        {
            return result.IsFailure ? Result.Failure<U>(result.Error) : continuation;
        }

        public Result<U> Map<U>(Func<T, Result<U>> map)
        {
            return result.IsFailure ? Result.Failure<U>(result.Error) : map(result.Value);
        }
        
        public Result<U> Map<U>(Func<Result<U>> map)
        {
            return result.IsFailure ? Result.Failure<U>(result.Error) : map();
        }
        
        public Result<U> Map<U>(Func<U> map)
        {
            return result.IsFailure ? Result.Failure<U>(result.Error) : map();
        }
    }
}