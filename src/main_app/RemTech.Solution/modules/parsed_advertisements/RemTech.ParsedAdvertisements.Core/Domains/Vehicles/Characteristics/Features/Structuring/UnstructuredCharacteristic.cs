using System.Diagnostics.CodeAnalysis;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class UnstructuredCharacteristic
{
    private readonly NotEmptyString _name;
    private readonly NotEmptyString _value;
    private readonly NotEmptyGuid _id;

    public UnstructuredCharacteristic(NotEmptyString name, NotEmptyString value)
    {
        _name = name;
        _value = value;
        _id = new NotEmptyGuid(Guid.NewGuid());
    }

    public UnstructuredCharacteristic(NotEmptyString name, NotEmptyString value, NotEmptyGuid id)
    {
        _name = name;
        _value = value;
        _id = id;
    }

    public string Name() => _name;
    
    public bool TryStructure([NotNullWhen(true)] out ValuedCharacteristic valued)
    {
        valued = null;
        IStructuringCharacteristic[] characteristics =
        [
            new StructuringBuCharacteristic(_name, _value, _id),
            new StructuringBucketCapacityCharacteristic(_name, _value, _id),
            new StructuringBucketControlTypeCharacteristic(_name, _value, _id),
            new StructuringEngineModelCharacteristic(_name, _value, _id),
            new StructuringEnginePowerCharacteristic(_name, _value, _id),
            new StructuringEngineTypeCharacteristic(_name, _value, _id),
            new StructuringEngineVolumeCharacteristic(_name, _value, _id),
            new StructuringFuelTankCapacityCharacteristic(_name, _value, _id),
            new StructuringLoadingHeightCharacteristic(_name, _value, _id),
            new StructuringLoadingWeightCharacteristic(_name, _value, _id),
            new StructuringReleaseYearCharacteristic(_name, _value, _id),
            new StructuringTorqueCharacteristic(_name, _value, _id),
            new StructuringTransportHoursCharacteristic(_name, _value, _id),
            new StructuringUnloadingHeightCharacteristic(_name, _value, _id),
            new StructuringVinCharacteristic(_name, _value, _id), 
            new StructuringWeightCharacteristic(_name, _value, _id),
        ];
        for (int i = 0; i < characteristics.Length; i++)
        {
            IStructuringCharacteristic attempt =  characteristics[i];
            if (attempt.Structure(out ValuedCharacteristic? result))
            {
                valued = result;
                return true;
            }
        }

        return false;
    }

    public Vehicle TryPut(Vehicle vehicle)
    {
        if (!TryStructure(out ValuedCharacteristic valued))
            return vehicle;
        return new Vehicle(vehicle,
            new VehicleCharacteristic(
                valued, 
                new VehicleCharacteristicValue(_value)));
    }
}