using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;

public sealed class WeightCharacteristicMeasureInspection(NotEmptyString name, NotEmptyString value)
    : ICharacteristicMeasureInspection
{
    public Characteristic Inspect(Characteristic ctx)
    {
        if (name != "Эксплуатационная масса")
            throw new OperationException("Характеристика не совместима.");
        return new Characteristic(
            new Characteristic(ctx, new CharacteristicMeasure("кг")),
            new VehicleCharacteristicValue(new OnlyDigitsString(value).Read())
        );
    }
}
