using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Transport.Vehicles;
using RemTech.ParsedAdvertisements.Core.Transport.Vehicles.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.ParsedItemSources.ParsedContainedItems;

public sealed record ParsedContainedItemId
{
    private readonly NotEmptyString _id;

    public ParsedContainedItemId(ParsedVehicle vehicle)
    {
        _id = vehicle.Identify();
    }

    public ParsedContainedItemId(ParsedVehicleIdentity identity)
    {
        _id = identity.ReadId();
    }

    public ParsedContainedItemId(ParsedVehicleId id)
    {
        _id = id;
    }
}
