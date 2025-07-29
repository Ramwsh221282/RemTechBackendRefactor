using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;

public sealed class EngineVolumeCharacteristicMeasureInspection(
    NotEmptyString name,
    NotEmptyString value
) : ICharacteristicMeasureInspection
{
    public Characteristic Inspect(Characteristic ctx)
    {
        if (name != "Объём двигателя")
            throw new OperationException("Характеристика не совместима.");
        FloatingNumberString floating = new(value);
        return new Characteristic(
            new Characteristic(ctx, new CharacteristicMeasure("л")),
            new VehicleCharacteristicValue(
                floating ? floating.Read() : new OnlyDigitsString(value).Read()
            )
        );
    }
}
