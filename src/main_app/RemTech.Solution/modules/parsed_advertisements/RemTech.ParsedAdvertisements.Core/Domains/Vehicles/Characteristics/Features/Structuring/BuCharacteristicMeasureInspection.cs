using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics.Features.Structuring;

public sealed class BuCharacteristicMeasureInspection(NotEmptyString name, NotEmptyString value) : ICharacteristicMeasureInspection
{
    public Characteristic Inspect(Characteristic ctx)
    {
        return name != "Б/у"
            ? throw new OperationException("Характеристика не совместима.")
            : new Characteristic(
                new Characteristic(ctx, new CharacteristicMeasure("Б/у")),
                new VehicleCharacteristicValue(value));
    }
}