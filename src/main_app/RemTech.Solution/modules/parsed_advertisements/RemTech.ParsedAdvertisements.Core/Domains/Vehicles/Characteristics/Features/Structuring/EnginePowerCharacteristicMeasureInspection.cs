﻿using System.Globalization;
using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class EnginePowerCharacteristicMeasureInspection(NotEmptyString name, NotEmptyString value)
    : ICharacteristicMeasureInspection
{
    private const double LsModifier = 1.36;

    public Characteristic Inspect(Characteristic ctx)
    {
        if (name == "Мощность двигателя" || name == "Мощность")
        {
            FloatingNumberString floatingNumberString = new(value);
            string formattedValue = floatingNumberString ? floatingNumberString.Read() : new OnlyDigitsString(value).Read();
            if (value.Contains("квт"))
            {
                double doubleValue = double.Parse(formattedValue) * LsModifier;
                formattedValue = doubleValue.ToString(CultureInfo.InvariantCulture);
            }

            Characteristic renamed = new Characteristic(ctx, new CharacteristicText("Мощность двигателя"));
            return new Characteristic(
                new Characteristic(renamed, new CharacteristicMeasure("лс")),
                new VehicleCharacteristicValue(formattedValue)
            );
        }
        throw new OperationException("Характеристика не совместима.");
    }
}