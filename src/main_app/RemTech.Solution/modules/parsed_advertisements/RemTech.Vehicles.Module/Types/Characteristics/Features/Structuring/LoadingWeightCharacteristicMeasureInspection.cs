using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

namespace RemTech.Vehicles.Module.Types.Characteristics.Features.Structuring;

public sealed class LoadingWeightCharacteristicMeasureInspection(
    NotEmptyString name,
    NotEmptyString value
) : ICharacteristicMeasureInspection
{
    public Characteristic Inspect(Characteristic ctx)
    {
        if (name != "Грузоподъёмность")
            throw new OperationException("Характеристика не совместима.");
        NotEmptyStringLength length = new(value);
        return new Characteristic(
            new Characteristic(ctx, new CharacteristicMeasure("кг")),
            new VehicleCharacteristicValue(
                length > 3
                    ? new OnlyDigitsString(value).Read()
                    : (int.Parse(new OnlyDigitsString(value).Read()) * 1000).ToString()
            )
        );
    }
}
