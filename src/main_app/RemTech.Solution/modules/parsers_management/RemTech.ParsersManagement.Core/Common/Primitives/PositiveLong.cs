using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Common.Primitives;

public sealed class PositiveLong
{
    private readonly long _value;

    public long Read() => _value;

    private PositiveLong(long value) => _value = value;

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
}
