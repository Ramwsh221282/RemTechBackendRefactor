using System.Reflection;

namespace RemTech.Functional.Extensions;

public class Optional
{
    public bool HasValue { get; }
    public bool NoValue => !HasValue;

    protected Optional(bool hasValue)
    {
        HasValue = hasValue;
    }

    public U Map<U>(Func<U> onHasValue, Func<U> onNoneValue)
    {
        return HasValue ? onHasValue() : onNoneValue();
    }

    public U Map<U>(U onHasValue, U onNoneValue)
    {
        return HasValue ? onHasValue : onNoneValue;
    }

    public static Optional<T> Some<T>(T value)
    {
        return new Optional<T>(value);
    }

    public static Optional<T> FromNullable<T>(T? value)
    {
        return value is null ? None<T>() : Some(value);
    }
    
    public static Optional<T> None<T>()
    {
        return new Optional<T>();
    }

    public static bool AllValuesExist(IEnumerable<Optional> optionals)
    {
        return !optionals.Any(optional => !optional.HasValue);
    }
    
    public static bool AllValuesExist<T>(T @object) where T : notnull
    {
        var fields = @object.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType.IsGenericType && 
                        f.FieldType.GetGenericTypeDefinition() == typeof(Optional<>));

        var optionals = fields.Select(f => (Optional)f.GetValue(@object)!);
        return AllValuesExist(optionals);
    }
}

public sealed class Optional<T> : Optional
{
    public T Value => HasValue
        ? field!
        : throw new InvalidOperationException($"Нельзя получить доступ к пустому значению {nameof(Optional<>)}");

    internal Optional(T value) : base(true)
    {
        Value = value;
    }

    internal Optional() : base(false)
    {
        Value = default;
    }

    public void ExecuteOnValue(Action action)
    {
        if (HasValue) action();
    }
    
    public void ExecuteOnValue(Action<T> action)
    {
        if (HasValue) action(Value);
    }

    public async Task ExecuteOnValueAsync(Func<Task> action)
    {
        if (HasValue) await action();
    }

    public async Task ExecuteOnValueAsync(Func<T, Task> action)
    {
        if (HasValue) await action(Value);
    }

    public static Optional<T> Some(T value)
    {
        return new Optional<T>(value);
    }

    public static Optional<T> None()
    {
        return new Optional<T>();
    }
}