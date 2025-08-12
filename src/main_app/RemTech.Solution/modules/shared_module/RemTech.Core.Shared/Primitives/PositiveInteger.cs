using RemTech.Core.Shared.Result;

namespace RemTech.Core.Shared.Primitives;

public readonly record struct PositiveInteger
{
    private readonly int _value = 0;

    public PositiveInteger(int? value) => _value = value ?? -1;

    public bool Same(PositiveInteger other) => _value == other._value;

    public int Read() => _value;

    public static implicit operator int(PositiveInteger positiveInteger) => positiveInteger._value;

    public static PositiveInteger New() => new(0);

    public static Status<PositiveInteger> New(int? value) =>
        value switch
        {
            null => new Error("Число было пустым.", ErrorCodes.Validation),
            not null when value.Value < 0 => new Error(
                "Число не должно быть отрицательным.",
                ErrorCodes.Validation
            ),
            _ => new PositiveInteger(value.Value),
        };

    public bool BiggerThan(PositiveInteger other) => _value > other._value;

    public bool LesserThan(PositiveInteger other) => _value < other._value;

    public override int GetHashCode() => _value.GetHashCode();

    public static implicit operator bool(PositiveInteger pos)
    {
        return pos >= 0;
    }

    public static implicit operator bool(PositiveInteger? pos)
    {
        return false;
    }
}
