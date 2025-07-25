using System.Diagnostics.CodeAnalysis;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class StructuringWeightCharacteristic(NotEmptyString name, NotEmptyString value, NotEmptyGuid id)
    : IStructuringCharacteristic
{
    public bool Structure([NotNullWhen(true)] out ValuedCharacteristic? ctx)
    {
        ctx = null;
        if (name != "Эксплуатационная масса") return false;
        string value1 = new OnlyDigitsString(value).Read();
        CharacteristicIdentity identity = new(new CharacteristicId(id), new CharacteristicText(name));
        CharacteristicMeasure measure = new(new NotEmptyString("кг"));
        ctx = new Characteristic(identity, measure).Print(new NotEmptyString(value1));
        return true;
    }
}