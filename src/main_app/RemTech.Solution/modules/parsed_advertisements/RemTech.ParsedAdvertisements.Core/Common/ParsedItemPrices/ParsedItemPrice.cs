namespace RemTech.ParsedAdvertisements.Core.Common.ParsedItemPrices;

public readonly record struct ParsedItemPrice : IParsedItemPrice
{
    private readonly IParsedItemPrice _price;

    public ParsedItemPrice(IParsedItemPrice price)
    {
        _price = price;
    }

    public PriceValue Value()
    {
        return _price.Value();
    }

    public bool UnderNds()
    {
        return _price.UnderNds();
    }
}
