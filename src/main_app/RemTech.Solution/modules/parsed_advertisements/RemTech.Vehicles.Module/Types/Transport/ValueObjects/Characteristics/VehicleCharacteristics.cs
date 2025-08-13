using RemTech.Core.Shared.Primitives;

namespace RemTech.Vehicles.Module.Types.Transport.ValueObjects.Characteristics;

internal sealed record VehicleCharacteristics
{
    private readonly HashSet<VehicleCharacteristic> _characteristics;

    public VehicleCharacteristics(IEnumerable<VehicleCharacteristic> characteristics)
    {
        _characteristics = new HashSet<VehicleCharacteristic>(characteristics);
    }

    public VehicleCharacteristic[] Read() => [.. _characteristics];

    public string MakeDocument()
    {
        string[] array = _characteristics
            .Select(c =>
                $"{(string)c.WhatCharacteristic().Identity.ReadText()} {(string)c.WhatValue()}"
            )
            .ToArray();
        return string.Join(' ', array);
    }

    public PositiveInteger Amount() => new(_characteristics.Count);
}
