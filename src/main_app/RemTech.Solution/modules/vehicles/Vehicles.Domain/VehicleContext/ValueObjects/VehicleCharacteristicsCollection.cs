namespace Vehicles.Domain.VehicleContext.ValueObjects;

public sealed record VehicleCharacteristicsCollection(
    IReadOnlyList<VehicleCharacteristic> Characteristics
);
