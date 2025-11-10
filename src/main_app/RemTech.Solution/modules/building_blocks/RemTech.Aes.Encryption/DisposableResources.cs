namespace RemTech.Aes.Encryption;

internal sealed class DisposableResources
{
    private readonly Dictionary<Type, (IDisposable, bool)> _disposables = [];

    public DisposableResources Add(IDisposable disposable)
    {
        const bool isDisposed = false;
        Type type = disposable.GetType();
        _disposables.Add(type, (disposable, isDisposed));
        return this;
    }

    public DisposableResources DisposeItem(IDisposable disposable)
    {
        Type type = disposable.GetType();
        if (!_disposables.TryGetValue(type, out (IDisposable, bool) value) || value.Item2)
            return this;
        value.Item1.Dispose();
        _disposables.Remove(type);
        return this;
    }

    public DisposableResources Disposed()
    {
        foreach ((IDisposable, bool) disposable in _disposables.Values)
            DisposeItem(disposable.Item1);
        return new DisposableResources();
    }
}