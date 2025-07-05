using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Primitives;

public sealed class PositiveInteger
{
    private readonly int _value;

    private PositiveInteger(int value) => _value = value;

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
}
