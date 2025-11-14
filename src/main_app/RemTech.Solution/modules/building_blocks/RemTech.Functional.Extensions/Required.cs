namespace RemTech.Functional.Extensions;

public static class Required
{
    public static Required<T> Sign<T>(T value)
    {
        return new Required<T>(value);
    }
    
    public static Required<T> Unsigned<T>()
    {
        return new Required<T>();
    }
}

public sealed class Required<T>
{
    private T Value => field ?? throw new InvalidOperationException($"{nameof(Required)} requires value of: {typeof(T).Name}");

    public bool Satisfied { get; }
    public bool NotSatisfied => !Satisfied;

    internal Required(T value)
    {
        Value = value;
        Satisfied = true;
    }

    internal Required()
    {
        Value = default;
        Satisfied = false;
    }

    public Optional<U> MapOptional<U>(Func<T, U> func)
    {
        return Satisfied ? Optional.Some(func(Value)) : Optional.None<U>(); 
    }
    
    public U Map<U>(Func<T, U> func)
    {
        return func(Value);
    }
    
    public Result<U> Map<U>(Func<T, U> mapFn, Error error)
    {
        return Satisfied ? mapFn(Value) : error;
    }
}

