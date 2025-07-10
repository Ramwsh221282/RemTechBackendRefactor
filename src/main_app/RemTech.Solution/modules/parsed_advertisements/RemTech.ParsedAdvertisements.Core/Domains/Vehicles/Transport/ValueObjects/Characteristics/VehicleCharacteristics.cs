using RemTech.Core.Shared.Extensions;
using RemTech.Core.Shared.Primitives;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Characteristics;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Transport.ValueObjects.Characteristics;

public sealed record VehicleCharacteristics
{
    private readonly HashSet<VehicleCharacteristic> _characteristics;

    public VehicleCharacteristics(IEnumerable<VehicleCharacteristic> characteristics)
    {
        _characteristics = new HashSet<VehicleCharacteristic>(characteristics);
    }

    public VehicleCharacteristic[] Read() => [.. _characteristics];

    public VehicleCharacteristic Characteristic(
        ICharacteristic characteristic,
        VehicleCharacteristicValue value
    )
    {
        MaybeBag<VehicleCharacteristic> existing = _characteristics.Maybe(c =>
            c.WhatCharacteristic()
                .Identify()
                .ReadText()
                .Equals(characteristic.Identify().ReadText())
        );
        if (existing.Any())
            return existing.Take();
        VehicleCharacteristic created = new(characteristic, value);
        _characteristics.Add(created);
        return created;
    }

    public PositiveInteger Amount() => new(_characteristics.Count);
}
