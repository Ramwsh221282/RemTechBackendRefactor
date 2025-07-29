using RemTech.Core.Shared.Exceptions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Types.Characteristics.ValueObjects;

namespace RemTech.ParsedAdvertisements.Core.Types.Characteristics.Features.Structuring;

public sealed class CharacteristicVeil
{
    private readonly NotEmptyString _name;
    private readonly NotEmptyString _value;
    private readonly NotEmptyGuid _id;

    public CharacteristicVeil(NotEmptyString name, NotEmptyString value)
    {
        _name = name;
        _value = value;
        _id = new NotEmptyGuid(Guid.NewGuid());
    }

    public string Name() => _name;

    public Characteristic Characteristic()
    {
        Characteristic ctx = new Characteristic(
            new CharacteristicIdentity(new CharacteristicId(_id), new CharacteristicText(_name))
        );
        ICharacteristicMeasureInspection[] characteristics =
        [
            new BuCharacteristicMeasureInspection(_name, _value),
            new BucketCapacityCharacteristicMeasureInspection(_name, _value),
            new BucketControlTypeCharacteristicMeasureInspection(_name, _value),
            new EngineModelCharacteristicMeasureInspection(_name, _value),
            new EnginePowerCharacteristicMeasureInspection(_name, _value),
            new EngineTypeCharacteristicMeasureInspection(_name, _value),
            new EngineVolumeCharacteristicMeasureInspection(_name, _value),
            new FuelTankCapacityCharacteristicMeasureInspection(_name, _value),
            new LoadingHeightCharacteristicMeasureInspection(_name, _value),
            new LoadingWeightCharacteristicMeasureInspection(_name, _value),
            new ReleaseYearCharacteristicMeasureInspection(_name, _value),
            new TorqueCharacteristicMeasureInspection(_name, _value),
            new TransportHoursCharacteristicMeasureInspection(_name, _value),
            new UnloadingHeightCharacteristicMeasureInspection(_name, _value),
            new VinCharacteristicMeasureInspection(_name, _value),
            new WeightCharacteristicMeasureInspection(_name, _value),
        ];
        foreach (ICharacteristicMeasureInspection inspection in characteristics)
        {
            try
            {
                return inspection.Inspect(ctx);
            }
            catch
            {
                // ignored
            }
        }

        throw new OperationException("Невозможно определить характеристику.");
    }
}
