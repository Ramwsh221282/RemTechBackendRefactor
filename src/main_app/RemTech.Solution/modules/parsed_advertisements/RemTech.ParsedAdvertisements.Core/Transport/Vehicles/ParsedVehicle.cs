using RemTech.ParsedAdvertisements.Core.Common.ParsedItemPrices;
using RemTech.ParsedAdvertisements.Core.Transport.Vehicles.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Transport.Vehicles;

public sealed class ParsedVehicle(
    ParsedVehicleIdentity identity,
    ParsedItemPrice price,
    ParsedVehicleText text,
    ParsedVehiclePhotos photos
)
{
    private readonly ParsedVehicleIdentity _identity = identity;
    private readonly ParsedItemPrice _price = price;
    private readonly ParsedVehicleText _text = text;
    private readonly ParsedVehiclePhotos _photos = photos;

    public ParsedVehicleIdentity Identify() => _identity;
}
