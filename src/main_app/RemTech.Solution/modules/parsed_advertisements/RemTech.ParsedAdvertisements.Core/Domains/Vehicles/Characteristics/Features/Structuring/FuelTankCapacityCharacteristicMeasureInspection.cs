using System.Diagnostics.CodeAnalysis;
using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.ValueObjects;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class FuelTankCapacityCharacteristicMeasureInspection(
    NotEmptyString name,
    NotEmptyString value)
    : ICharacteristicMeasureInspection
{
    public Characteristic Inspect(Characteristic ctx)
    {
        return name != "Объём топливного бака"
            ? throw new OperationException("Характеристика не совместима.")
            : new Characteristic(
                new Characteristic(ctx, new CharacteristicMeasure("л")),
                new VehicleCharacteristicValue(new OnlyDigitsString(value).Read()));
    }
}