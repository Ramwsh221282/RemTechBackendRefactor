using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed class VehiclePrice
{
    private readonly ParsedItemPrice _price;

    public VehiclePrice(ParsedItemPrice price)
    {
        _price = price;
    }

    public ParsedItemPrice Read() => _price;
}
