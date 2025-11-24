namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public class Try
{
    public static Try<T> For<T>(Func<T> factory)
    {
        return new Try<T>(factory);
    }
}

public class Try<T>
{
    private readonly bool _success;
    private readonly Func<T> _factory;
    private readonly Error _error;

    internal Try(Func<T> factory)
    {
        _factory = factory;
        _success = false;
        _error = Error.NoError();
    }


    internal Try(Try<T> origin, Error error)
    {
        _success = origin._success;
        _factory = origin._factory;
        _error = error;
    }

    public Try<T> BindError(Error error)
    {
        return new Try<T>(this, error);
    }

    public Result<T> Attempt()
    {
        try
        {
            var result = _factory();
            return Result.Success(result);
        }
        catch
        {
            return Result.Failure<T>(_error);
        }
    }
}

public class AsyncTry
{
    private readonly bool _success;
    private readonly Func<Task> _factory;
    private readonly Error _error;

    internal AsyncTry(Func<Task> factory)
    {
        _factory = factory;
        _success = false;
        _error = Error.NoError();
    }

    internal AsyncTry()
    {
        _success = true;
        _error = Error.NoError();
        _factory = async () => await Task.Yield();
    }

    public void Effect(Action onSuccess, Action onFailure)
    {
        if (_success)
        {
            onSuccess();
            return;
        }
        
        onFailure();
    }
    
    internal AsyncTry(Error error)
    {
        _success = false;
        _error = error;
        _factory = async () => await Task.Yield();
    }
    
    internal AsyncTry(AsyncTry origin, Error error)
    {
        _success = origin._success;
        _factory = origin._factory;
        _error = error;
    }

    public static AsyncTry For(Func<Task> factory)
    {
        return new AsyncTry(factory);
    }
    
    public AsyncTry BindError(Error error)
    {
        return new AsyncTry(this, error);
    }

    public async Task<AsyncTry> Attempt()
    {
        try
        {
            await _factory();
            return new AsyncTry();
        }
        catch
        {
            return new AsyncTry(_error);
        }
    }
}