using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;

internal sealed class BucketCapacityCharacteristicMeasureInspection(
    NotEmptyString name,
    NotEmptyString value
) : ICharacteristicMeasureInspection
{
    public Characteristic Inspect(Characteristic ctx)
    {
        if (name != "Объём ковша")
            throw new OperationException("Характеристика не совместима.");
        FloatingNumberString floating = new(value);
        return new Characteristic(
            new Characteristic(ctx, new CharacteristicMeasure("л")),
            new VehicleCharacteristicValue(
                new NotEmptyString(floating ? floating.Read() : new OnlyDigitsString(value).Read())
            )
        );
    }
}
