using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed class ParsedTransportPrice
{
    private readonly ParsedItemPrice _price;

    public ParsedTransportPrice(ParsedItemPrice price)
    {
        _price = price;
    }
}
