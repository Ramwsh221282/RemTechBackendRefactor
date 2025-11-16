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

public struct AsyncResult<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public Error Error { get; }

    public AsyncResult(T value) => (IsSuccess, Value, Error) = (true, value, null);
    public AsyncResult(Error error) => (IsSuccess, Value, Error) = (false, default, error);

    public async Task<TResult> Match<TResult>(Func<T, Task<TResult>> onSuccess, Func<Error, Task<TResult>> onError) =>
        IsSuccess ? await onSuccess(Value) : await onError(Error);
}

public static class AsyncResultExtensions
{
    public static async Task<AsyncResult<U>> SelectAsync<T, U>(
        this Task<AsyncResult<T>> source,
        Func<T, U> selector) =>
        (await source).Select(selector);

    public static AsyncResult<U> Select<T, U>(
        this AsyncResult<T> source,
        Func<T, U> selector) =>
        source.IsSuccess 
            ? new AsyncResult<U>(selector(source.Value)) 
            : new AsyncResult<U>(source.Error);

    public static async Task<AsyncResult<U>> SelectManyAsync<T, U>(
        this Task<AsyncResult<T>> source,
        Func<T, Task<AsyncResult<U>>> selector) =>
        await (await source).SelectManyAsync(selector);

    public static async Task<AsyncResult<U>> SelectManyAsync<T, U>(
        this AsyncResult<T> source,
        Func<T, Task<AsyncResult<U>>> selector) =>
        source.IsSuccess 
            ? await selector(source.Value) 
            : new AsyncResult<U>(source.Error);
    
    public static async Task<AsyncResult<V>> SelectMany<T, U, V>(
        this Task<AsyncResult<T>> source,
        Func<T, Task<AsyncResult<U>>> collectionSelector,
        Func<T, U, V> resultSelector) =>
        await (await source).SelectManyAsync(collectionSelector, resultSelector);

    public static async Task<AsyncResult<V>> SelectManyAsync<T, U, V>(
        this AsyncResult<T> source,
        Func<T, Task<AsyncResult<U>>> collectionSelector,
        Func<T, U, V> resultSelector) =>
        source.IsSuccess
            ? await collectionSelector(source.Value).ContinueWith(async task =>
                    task.Result.IsSuccess 
                        ? new AsyncResult<V>(resultSelector(source.Value, task.Result.Value))
                        : new AsyncResult<V>(task.Result.Error))
                .Unwrap()
            : new AsyncResult<V>(source.Error);
}

public static class ResultModuleExtensions
{
    extension<T>(Result<T> from)
    {
        public Result<U> Select<U>(Func<T, U> resultFn)
        {
            return from.IsFailure ? Result.Failure<U>(from.Error) : resultFn(from.Value);
        }

        public Result<U> SelectMany<V, U>(
            Func<T, Result<V>> collectionSelector,
            Func<T, V, Result<U>> resultSelector)
        {
            if (!from.IsSuccess) return Result.Failure<U>(from.Error);
            Result<V> result = collectionSelector(from.Value);
            return result.Select(v => resultSelector(from, v));
        }
    }
}