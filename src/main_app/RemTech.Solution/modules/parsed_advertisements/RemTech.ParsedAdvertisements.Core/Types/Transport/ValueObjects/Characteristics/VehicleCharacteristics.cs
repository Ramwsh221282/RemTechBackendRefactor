using RemTech.Core.Shared.Primitives;

namespace RemTech.ParsedAdvertisements.Core.Types.Transport.ValueObjects.Characteristics;

public sealed record VehicleCharacteristics
{
    private readonly HashSet<VehicleCharacteristic> _characteristics;

    public VehicleCharacteristics(IEnumerable<VehicleCharacteristic> characteristics)
    {
        _characteristics = new HashSet<VehicleCharacteristic>(characteristics);
    }

    public VehicleCharacteristic[] Read() => [.. _characteristics];

    public PositiveInteger Amount() => new(_characteristics.Count);
}
