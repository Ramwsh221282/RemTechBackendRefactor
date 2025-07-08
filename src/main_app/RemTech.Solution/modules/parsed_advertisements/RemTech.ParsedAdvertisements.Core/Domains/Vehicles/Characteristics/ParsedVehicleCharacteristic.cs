using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public sealed class ParsedVehicleCharacteristic(ParsedVehicleCharacteristicIdentity identity)
{
    private readonly ParsedVehicleCharacteristicIdentity _identity = identity;

    public ParsedVehicleCharacteristic(ParsedVehicleCharacteristicText text)
        : this(new ParsedVehicleCharacteristicIdentity(text)) { }

    public ParsedVehicleCharacteristic(
        ParsedVehicleCharacteristicId id,
        ParsedVehicleCharacteristicText text
    )
        : this(new ParsedVehicleCharacteristicIdentity(id, text)) { }

    public ParsedVehicleCharacteristicIdentity Identify() => _identity;
}
