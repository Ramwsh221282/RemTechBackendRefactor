using System.Diagnostics.CodeAnalysis;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class StructuringBuCharacteristic : IStructuringCharacteristic
{
    private readonly NotEmptyString _name;
    private readonly NotEmptyString _value;

    public StructuringBuCharacteristic(NotEmptyString name, NotEmptyString value)
    {
        _name = name;
        _value = value;
    }
    
    public bool Structure([NotNullWhen(true)] out ValuedCharacteristic? ctx)
    {
        ctx = null;
        if (_name != "Б/у") return false;
        CharacteristicMeasure measure = new("Б/у");
        CharacteristicIdentity identity = new(new CharacteristicId(Guid.NewGuid()), new CharacteristicText(_name));
        ctx = new Characteristic(identity, measure).Print(new NotEmptyString(_value));
        return true;
    }
}