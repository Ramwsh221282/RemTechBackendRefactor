using RemTech.Core.Shared.Enumerables;
using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext.Errors;

namespace Vehicles.Domain.VehicleContext.ValueObjects;

public sealed record VehicleCharacteristicsCollection
{
    public IReadOnlyList<VehicleCharacteristic> Characteristics { get; }

    private VehicleCharacteristicsCollection(IEnumerable<VehicleCharacteristic> characteristics) =>
        Characteristics = [.. characteristics];

    public static Result<VehicleCharacteristicsCollection> Create(
        IEnumerable<VehicleCharacteristic> characteristics
    ) =>
        !characteristics.HasRepeatableValues(c => c.Name, out VehicleCharacteristic[] repeatable)
            ? new VehicleCharacteristicsCollection(characteristics)
            : new VehicleCharacteristicsRepeatableError(repeatable);
}
