namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

public sealed record VehicleCharacteristics
{
    private readonly HashSet<VehicleCharacteristic> _characteristics;

    public VehicleCharacteristics(IEnumerable<VehicleCharacteristic> characteristics)
    {
        _characteristics = new HashSet<VehicleCharacteristic>(characteristics);
    }

    public VehicleCharacteristic[] Read() => [.. _characteristics];
}
