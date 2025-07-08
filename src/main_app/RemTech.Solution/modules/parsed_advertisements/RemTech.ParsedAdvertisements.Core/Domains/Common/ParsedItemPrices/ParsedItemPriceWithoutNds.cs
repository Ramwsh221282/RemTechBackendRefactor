using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

public readonly record struct ParsedItemPriceWithoutNds : IParsedItemPrice
{
    private readonly PriceValue _value;

    public ParsedItemPriceWithoutNds(PriceValue value)
    {
        _value = value;
    }

    public PriceValue Value()
    {
        return _value;
    }

    public bool UnderNds()
    {
        return false;
    }

    public static implicit operator PositiveLong(ParsedItemPriceWithoutNds parsedItemPrice)
    {
        return parsedItemPrice._value;
    }

    public static implicit operator long(ParsedItemPriceWithoutNds parsedItemPrice)
    {
        return parsedItemPrice._value;
    }

    public static implicit operator bool(ParsedItemPriceWithoutNds parsedItemPrice)
    {
        return parsedItemPrice._value;
    }
}
