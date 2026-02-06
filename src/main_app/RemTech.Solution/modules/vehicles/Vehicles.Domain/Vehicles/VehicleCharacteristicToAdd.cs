using Vehicles.Domain.Characteristics;

namespace Vehicles.Domain.Vehicles;

/// <summary>
/// Характеристика транспортного средства для добавления.
/// </summary>
/// <param name="Characteristic">Характеристика транспортного средства.</param>
/// <param name="Value">Значение характеристики транспортного средства.</param>
public sealed record VehicleCharacteristicToAdd(Characteristic Characteristic, VehicleCharacteristicValue Value);
