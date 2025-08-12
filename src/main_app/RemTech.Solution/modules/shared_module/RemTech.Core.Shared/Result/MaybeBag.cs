namespace RemTech.Core.Shared.Result;

public class MaybeBag
{
    private bool _has;

    public bool Any() => _has;

    public MaybeBag() => _has = false;

    public MaybeBag(bool has) => _has = has;

    public MaybeBag Put() => new(true);

    public MaybeBag Drop() => new();
}

public class MaybeBag<TValue> : MaybeBag
{
    private readonly TValue? _value;

    public MaybeBag(TValue value)
        : base(true) => _value = value;

    public MaybeBag()
        : base(false) => _value = default!;

    public MaybeBag<TValue> Put(TValue value) => new(value);

    public new MaybeBag<TValue> Drop() => new();

    public MaybeBag<TValue> Put(Status<TValue> status)
    {
        MaybeBag<TValue> maybeBag = new();
        return status.IsFailure ? maybeBag : maybeBag.Put(status.Value);
    }

    public MaybeBag<TValue> MaybePut<T>(T value, Func<T, bool> predicate, Func<T, TValue> converter)
    {
        MaybeBag<TValue> bag = new();
        return predicate(value) ? bag.Put(converter(value)) : bag;
    }

    public TValue Take() =>
        _value ?? throw new ApplicationException("Попытка получить ничего из Maybe.");

    public static implicit operator MaybeBag<TValue>(TValue value) => new(value);

    public static implicit operator MaybeBag<TValue>(Status<TValue> status) =>
        status.IsFailure ? new MaybeBag<TValue>() : new MaybeBag<TValue>(status.Value);

    public static implicit operator TValue(MaybeBag<TValue> maybeBag) => maybeBag._value!;
}

public static class MaybeExtensions
{
    public static MaybeBag<U> Adapt<T, U>(this MaybeBag<T> value, Func<T, U> selector) =>
        value switch
        {
            null => new MaybeBag<U>(),
            not null when value.Any() == false => new MaybeBag<U>(),
            _ => new MaybeBag<U>().Put(selector(value)),
        };
}
