using ParsedAdvertisements.Domain.VehicleContext.Entities;
using RemTech.Core.Shared.Result;

namespace ParsedAdvertisements.Domain.VehicleContext.ValueObjects;

public sealed class VehicleCharacteristicsList
{
    private readonly HashSet<VehicleCharacteristic> _characteristics;

    private VehicleCharacteristicsList(IEnumerable<VehicleCharacteristic> characteristics) =>
        _characteristics = [.. characteristics];

    public VehicleCharacteristicsList() => _characteristics = [];

    public Status<VehicleCharacteristic> Add(VehicleCharacteristic characteristic)
    {
        if (!_characteristics.Add(characteristic))
            return Error.Conflict("У техники уже есть такая характеристика");
        return characteristic;
    }

    public static Status<VehicleCharacteristicsList> Create(
        IEnumerable<VehicleCharacteristic> characteristics
    )
    {
        var array = characteristics.ToArray();
        var distinct = characteristics.DistinctBy(c => c.CharacteristicId).ToArray();
        if (distinct.Length != array.Length)
            return Error.Validation("Характеристики техники должны быть уникальными.");
        return new VehicleCharacteristicsList(characteristics);
    }
}
