using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.ParsedAdvertisements.Core.Types.Characteristics.Features.Structuring;

public sealed class LoadingHeightCharacteristicMeasureInspection(
    NotEmptyString name,
    NotEmptyString value
) : ICharacteristicMeasureInspection
{
    public Characteristic Inspect(Characteristic ctx)
    {
        return name != "Высота подъёма"
            ? throw new OperationException("Характеристика не совместима.")
            : new Characteristic(
                new Characteristic(ctx, new CharacteristicMeasure("мм")),
                new VehicleCharacteristicValue(new OnlyDigitsString(value).Read())
            );
    }
}
