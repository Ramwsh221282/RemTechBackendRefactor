using RemTech.Result.Library;

namespace RemTech.Core.Shared.Primitives;

public sealed class PositiveLong
{
    private readonly long _value;

    public long Read() => _value;

    public PositiveLong(long? value)
    {
        _value = value ?? -1;
    }

    public bool Same(PositiveLong other) => _value == other._value;

    public static implicit operator long(PositiveLong positiveLong) => positiveLong._value;

    public static PositiveLong New() => new(0);

    public static Status<PositiveLong> New(long? value) =>
        value switch
        {
            null => new Error("Число было пустым.", ErrorCodes.Validation),
            not null when value.Value < 0 => new Error(
                "Число не должно быть отрицательным.",
                ErrorCodes.Validation
            ),
            _ => new PositiveLong(value.Value),
        };

    public override bool Equals(object? obj) =>
        obj switch
        {
            null => false,
            PositiveLong pl => pl._value == _value,
            _ => false,
        };

    public override int GetHashCode() => _value.GetHashCode();

    public static implicit operator bool(PositiveLong pos)
    {
        return pos._value >= 0;
    }
}
