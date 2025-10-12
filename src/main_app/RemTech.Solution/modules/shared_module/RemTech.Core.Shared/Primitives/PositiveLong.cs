using RemTech.Result.Pattern;

namespace RemTech.Core.Shared.Primitives;

public readonly record struct PositiveLong
{
    private readonly long _value = 0;

    public long Read() => _value;

    public PositiveLong(long? value)
    {
        _value = value ?? -1;
    }

    public bool Same(PositiveLong other) => _value == other._value;

    public static implicit operator long(PositiveLong positiveLong) => positiveLong._value;

    public static PositiveLong New() => new(0);

    public static Result<PositiveLong> New(long? value) =>
        value switch
        {
            null => new Error("Число было пустым.", ErrorCodes.Validation),
            not null when value.Value < 0 => new Error(
                "Число не должно быть отрицательным.",
                ErrorCodes.Validation
            ),
            _ => new PositiveLong(value.Value),
        };

    public override int GetHashCode() => _value.GetHashCode();

    public static implicit operator bool(PositiveLong pos)
    {
        return pos._value >= 0;
    }

    public static implicit operator bool(PositiveLong? pos)
    {
        return false;
    }
}
