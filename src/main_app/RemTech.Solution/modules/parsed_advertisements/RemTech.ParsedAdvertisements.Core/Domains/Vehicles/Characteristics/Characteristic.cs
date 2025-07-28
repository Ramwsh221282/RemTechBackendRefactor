using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Adapters.Storage.Postgres;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;

public sealed class Characteristic : ICharacteristic
{
    private readonly CharacteristicIdentity _identity;
    private readonly CharacteristicMeasure _measure;
    private readonly VehicleCharacteristicValue _value;

    public Characteristic(
        CharacteristicIdentity identity,
        CharacteristicMeasure measure,
        VehicleCharacteristicValue value)
    {
        _identity = identity;
        _measure = measure;
        _value = value;
    }

    public Characteristic(Characteristic origin, VehicleCharacteristicValue value) : this(origin)
    {
        _value = value;
    }

    public Characteristic(Characteristic origin, CharacteristicText otherName) : this(origin)
    {
        _identity = new CharacteristicIdentity(new CharacteristicId(_identity.ReadId()), otherName);
    }
    
    public Characteristic(CharacteristicIdentity identity)
    {
        _identity = identity;
        _measure = new CharacteristicMeasure();
        _value = new VehicleCharacteristicValue(new NotEmptyString(string.Empty));
    }

    public Characteristic(CharacteristicIdentity identity, CharacteristicMeasure measure)
    {
        _identity = identity;
        _measure = measure;
        _value = new VehicleCharacteristicValue(new NotEmptyString(string.Empty));
    }

    public Characteristic(Characteristic origin)
    {
        _identity = origin._identity;
        _measure = origin._measure;
        _value = origin._value;
    }

    public Characteristic(Characteristic origin, CharacteristicMeasure measure)
    : this(origin)
    {
        _measure = measure;
    }

    public CharacteristicIdentity Identity => _identity;
    
    public Vehicle ToVehicle(Vehicle vehicle)
    {
        return !_value
            ? throw new OperationException("Нельзя передать характеристику технике без значения характеристики.")
            : new Vehicle(vehicle, new VehicleCharacteristic(this, _value));
    }

    public UniqueCharacteristics Print(UniqueCharacteristics storage)
    {
        string name = _identity.ReadText();
        return storage.With(new NotEmptyString(name), this);
    }

    public PgCharacteristicToStoreCommand ToStoreCommand() =>
        new(_identity.ReadId(), _identity.ReadText(), _measure.Read());

    public PgCharacteristicFromStoreCommand FromStoreCommand() => new(_identity.ReadText(), _value);

    public ParametrizingPgCommand VehicleCtxPgCommand(VehicleCharacteristicValue value, int index, ParametrizingPgCommand command)
    {
        string ctxValue = value;
        Guid ctxId = _identity.ReadId();
        string ctxName = _identity.ReadText();
        string measure = _measure.Read();
        return command
            .With($"@ctx_id_{index}", ctxId)
            .With($"@ctx_name_{index}", ctxName)
            .With($"@ctx_value_{index}", ctxValue)
            .With($"@ctx_measure_{index}", measure);
    }
    
    public string Measure() => _measure.Read();
    
    public string NameValueString(NotEmptyString value)
    {
        string name = _identity.ReadText();
        return $"{name} {(string)value}";
    }
}
