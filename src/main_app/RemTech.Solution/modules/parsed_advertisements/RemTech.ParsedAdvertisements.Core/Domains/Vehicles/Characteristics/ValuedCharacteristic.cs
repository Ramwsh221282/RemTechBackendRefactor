using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public sealed class ValuedCharacteristic : ICharacteristic
{
    private readonly ICharacteristic _origin;
    private readonly NotEmptyString _value;

    public ValuedCharacteristic(ICharacteristic origin, NotEmptyString value)
    {
        _origin = origin;
        _value = value;
    }

    public CharacteristicIdentity Identify() =>
        _origin.Identify();

    public CharacteristicMeasure Measure() =>
        _origin.Measure();

    public PgCharacteristicToStoreCommand StoreCommand() =>
        new(_origin.Identify().ReadId(), _origin.Identify().ReadText(), _value);

    public PgCharacteristicFromStoreCommand FromStoreCommand() =>
        new(_origin.Identify().ReadText());

    public Vehicle ToVehicle(Vehicle vehicle)
    {
        return new Vehicle(vehicle, 
            new VehicleCharacteristic(this,
                new VehicleCharacteristicValue(_value)));
    }
}