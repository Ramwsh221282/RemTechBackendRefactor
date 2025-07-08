using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

public readonly record struct ParsedItemPriceWithNds : IParsedItemPrice
{
    private readonly PriceValue _value;

    public ParsedItemPriceWithNds(PriceValue value)
    {
        _value = value;
    }

    public PriceValue Value()
    {
        return _value;
    }

    public bool UnderNds()
    {
        return true;
    }

    public static implicit operator PositiveLong(ParsedItemPriceWithNds parsedItemPrice)
    {
        return parsedItemPrice._value;
    }

    public static implicit operator long(ParsedItemPriceWithNds parsedItemPrice)
    {
        return parsedItemPrice._value;
    }

    public static implicit operator bool(ParsedItemPriceWithNds parsedItemPrice)
    {
        return parsedItemPrice._value;
    }
}
