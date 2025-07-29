using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Prices;

public readonly record struct PriceValue
{
    private readonly PositiveLong _value;

    public PriceValue(PositiveLong value)
    {
        _value = value;
    }

    public PriceValue(long? value)
        : this(new PositiveLong(value)) { }

    public static implicit operator PositiveLong(PriceValue priceValue) => priceValue._value;

    public static implicit operator long(PriceValue value) => value._value;

    public static implicit operator bool(PriceValue priceValue) => priceValue._value;
}
