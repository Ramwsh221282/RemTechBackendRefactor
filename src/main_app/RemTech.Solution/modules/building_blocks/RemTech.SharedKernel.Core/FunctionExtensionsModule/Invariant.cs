namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public class Invariant
{
    public static Invariant<T> For<T>(T value, Func<T, bool> predicate)
    {
        return new Invariant<T>(value, predicate);
    }
}

public class Invariant<T>
{
    private readonly Error _error;
    private readonly bool _satisfied;
    private readonly T? _value;

    internal Invariant(T value, Func<T, bool> predicate)
    {
        _satisfied = predicate(value);
        _error = Error.NoError();
        _value = value;
    }

    internal Invariant(Error error)
    {
        _error = error;
        _satisfied = false;
        _value = default;
    }

    internal Invariant(Invariant<T> origin, Error error)
    {
        _satisfied = origin._satisfied;
        _value = origin._value;
        _error = error;
    }

    internal Invariant(Invariant<T> origin, Func<T, bool> predicate)
    {
        if (origin._satisfied)
            _satisfied = predicate(origin._value!);
        _error = origin._error;
    }

    public static Invariant<T> For(T value, Func<T, bool> predicate)
    {
        return new Invariant<T>(value, predicate);
    }

    public Invariant<T> BindError(Error error)
    {
        return _satisfied ? this : new Invariant<T>(this, error);
    }

    public Invariant<U> SwitchTo<U>(U value, Func<U, bool> predicate)
    {
        return !_satisfied ? new Invariant<U>(_error) : new Invariant<U>(value, predicate);
    }

    public Result<U> Map<U>(Func<U> success)
    {
        return !_satisfied ? Result.Failure<U>(_error) : success();
    }
}