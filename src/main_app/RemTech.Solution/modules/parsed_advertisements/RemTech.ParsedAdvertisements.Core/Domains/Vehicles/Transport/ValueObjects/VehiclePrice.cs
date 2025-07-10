using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects;

public sealed class VehiclePrice
{
    private readonly ItemPrice _price;

    public VehiclePrice(ItemPrice price)
    {
        _price = price;
    }

    public ItemPrice Read() => _price;
}
