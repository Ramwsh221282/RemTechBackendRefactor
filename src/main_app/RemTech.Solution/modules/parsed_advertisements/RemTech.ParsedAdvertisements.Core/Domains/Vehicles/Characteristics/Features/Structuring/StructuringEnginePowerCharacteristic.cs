using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class StructuringEnginePowerCharacteristic : IStructuringCharacteristic
{
    private readonly NotEmptyString _name;
    private readonly NotEmptyString _value;
    private readonly NotEmptyGuid _id;
    private const double LsModifier = 1.36;

    public StructuringEnginePowerCharacteristic(NotEmptyString name, NotEmptyString value, NotEmptyGuid id)
    {
        _name = name;
        _value = value;
        _id = id;
    }
    
    public bool Structure([NotNullWhen(true)] out ValuedCharacteristic? ctx)
    {
        ctx = null;
        if (_name != "Мощность двигателя") return false;
        FloatingNumberString floatingNumberString = new(_value);
        string value = floatingNumberString ? floatingNumberString.Read() : new OnlyDigitsString(_value).Read();
        if (_value.Contains("квт"))
        {
            double doubleValue = double.Parse(value) * LsModifier;
            value = doubleValue.ToString(CultureInfo.InvariantCulture);
        }

        CharacteristicMeasure measure = new("лс");
        CharacteristicIdentity identity = new(new CharacteristicId(_id), new CharacteristicText(_name));
        ctx = new Characteristic(identity, measure).Print(new NotEmptyString(value));
        return true;
    }
}