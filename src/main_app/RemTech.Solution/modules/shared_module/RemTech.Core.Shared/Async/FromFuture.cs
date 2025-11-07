namespace RemTech.Core.Shared.Async;

public sealed class FromFuture<T>(Func<Task<T>> func)
{
    private readonly Func<Task<T>> _func = func;
    public async Task<T> Read() => await _func();

    public FromFuture(FromFuture<T> future) : this(future._func)
    {
    }
}

public sealed class Future(Func<Task> func)
{
    private readonly Func<Task> _func = func;
    public async Task Complete() => await _func();

    public Future(Future future) : this(future._func)
    {
    }
}