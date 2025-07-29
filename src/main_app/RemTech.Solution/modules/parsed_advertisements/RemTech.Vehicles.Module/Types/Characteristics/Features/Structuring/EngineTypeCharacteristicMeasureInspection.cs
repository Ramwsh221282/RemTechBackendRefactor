using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;

public sealed class EngineTypeCharacteristicMeasureInspection(
    NotEmptyString name,
    NotEmptyString value
) : ICharacteristicMeasureInspection
{
    public Characteristic Inspect(Characteristic ctx)
    {
        return name != "Тип двигателя"
            ? throw new OperationException("Характеристика не совместима.")
            : new Characteristic(
                new Characteristic(ctx, new CharacteristicMeasure("тип")),
                new VehicleCharacteristicValue(value)
            );
    }
}
