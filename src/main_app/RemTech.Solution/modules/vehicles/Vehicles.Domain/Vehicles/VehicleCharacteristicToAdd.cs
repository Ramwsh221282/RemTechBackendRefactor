using Vehicles.Domain.Characteristics;

namespace Vehicles.Domain.Vehicles;

public sealed record VehicleCharacteristicToAdd(Characteristic Characteristic, VehicleCharacteristicValue Value);