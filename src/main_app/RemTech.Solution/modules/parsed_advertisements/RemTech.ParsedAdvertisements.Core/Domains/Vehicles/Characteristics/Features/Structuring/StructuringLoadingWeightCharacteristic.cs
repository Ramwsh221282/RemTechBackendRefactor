using System.Diagnostics.CodeAnalysis;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class StructuringLoadingWeightCharacteristic(NotEmptyString name, NotEmptyString value, NotEmptyGuid id)
    : IStructuringCharacteristic
{
    public bool Structure([NotNullWhen(true)] out ValuedCharacteristic? ctx)
    {
        ctx = null;
        if (name != "Грузоподъёмность") return false;
        NotEmptyStringLength length = new(value);
        string value1 = length > 3
            ? new OnlyDigitsString(value).Read()
            : (int.Parse(new OnlyDigitsString(value).Read()) * 1000).ToString();
        CharacteristicMeasure measure = new("кг");
        CharacteristicIdentity identity = new(new CharacteristicId(id), new CharacteristicText(name));
        ctx = new Characteristic(identity, measure).Print(new NotEmptyString(value1));
        return true;
    }
}