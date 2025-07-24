using System.Diagnostics.CodeAnalysis;
using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class UnstructuredCharacteristic
{
    private readonly NotEmptyString _name;
    private readonly NotEmptyString _value;

    public UnstructuredCharacteristic(NotEmptyString name, NotEmptyString value)
    {
        _name = name;
        _value = value;
    }

    public string Name() => _name;
    
    public bool TryStructure([NotNullWhen(true)] out ValuedCharacteristic valued)
    {
        valued = null;
        IStructuringCharacteristic[] characteristics =
        [
            new StructuringBuCharacteristic(_name, _value),
            new StructuringBucketCapacityCharacteristic(_name, _value),
            new StructuringBucketControlTypeCharacteristic(_name, _value),
            new StructuringEngineModelCharacteristic(_name, _value),
            new StructuringEnginePowerCharacteristic(_name, _value),
            new StructuringEngineTypeCharacteristic(_name, _value),
            new StructuringEngineVolumeCharacteristic(_name, _value),
            new StructuringFuelTankCapacityCharacteristic(_name, _value),
            new StructuringLoadingHeightCharacteristic(_name, _value),
            new StructuringLoadingWeightCharacteristic(_name, _value),
            new StructuringReleaseYearCharacteristic(_name, _value),
            new StructuringTorqueCharacteristic(_name, _value),
            new StructuringTransportHoursCharacteristic(_name, _value),
            new StructuringUnloadingHeightCharacteristic(_name, _value),
            new StructuringVinCharacteristic(_name, _value), 
            new StructuringWeightCharacteristic(_name, _value),
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
}