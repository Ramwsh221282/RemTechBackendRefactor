using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Characteristics.ValueObjects;
using RemTech.Vehicles.Module.Types.Transport;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Characteristics;

internal sealed class Characteristic(
    CharacteristicIdentity identity,
    VehicleCharacteristicValue value
) : ICharacteristic
{
    private readonly CharacteristicIdentity _identity = identity;
    private readonly VehicleCharacteristicValue _value = value;

    public Characteristic(Characteristic origin, VehicleCharacteristicValue value)
        : this(origin)
    {
        _value = value;
    }

    public Characteristic(Characteristic origin, CharacteristicText otherName)
        : this(origin)
    {
        _identity = new CharacteristicIdentity(new CharacteristicId(_identity.ReadId()), otherName);
    }

    public Characteristic(CharacteristicIdentity identity)
        : this(identity, new VehicleCharacteristicValue(new NotEmptyString(string.Empty))) { }

    public Characteristic(Characteristic origin)
        : this(origin._identity, origin._value) { }

    public CharacteristicIdentity Identity => _identity;

    public Vehicle ToVehicle(Vehicle vehicle)
    {
        return !_value
            ? throw new OperationException(
                "Нельзя передать характеристику технике без значения характеристики."
            )
            : vehicle.Accept(new VehicleCharacteristic(this, _value));
    }
}
