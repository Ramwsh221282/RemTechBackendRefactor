using System.Diagnostics.CodeAnalysis;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class StructuringEngineVolumeCharacteristic : IStructuringCharacteristic
{
    private readonly NotEmptyString _name;
    private readonly NotEmptyString _value;
    private readonly NotEmptyGuid _id;

    public StructuringEngineVolumeCharacteristic(NotEmptyString name, NotEmptyString value, NotEmptyGuid id)
    {
        _name = name;
        _value = value;
        _id = id;
    }

    public bool Structure([NotNullWhen(true)] out ValuedCharacteristic? ctx)
    {
        ctx = null;
        if (_name != "Объём двигателя") return false;
        FloatingNumberString floating = new(_value);
        string value = floating ? floating.Read() :  new OnlyDigitsString(_value).Read();
        CharacteristicMeasure measure = new("л");
        CharacteristicIdentity identity = new(new CharacteristicId(_id), new CharacteristicText(_name));
        ctx = new Characteristic(identity, measure).Print(new NotEmptyString(value));
        return true;
    }
}