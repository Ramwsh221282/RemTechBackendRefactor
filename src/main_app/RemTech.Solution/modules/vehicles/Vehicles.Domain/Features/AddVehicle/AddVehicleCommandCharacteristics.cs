namespace Vehicles.Domain.Features.AddVehicle;

/// <summary>
/// Характеристики транспортного средства для команды добавления транспортного средства.
/// </summary>
/// <param name="Name">Название характеристики транспортного средства.</param>
/// <param name="Value">Значение характеристики транспортного средства.</param>
public sealed record AddVehicleCommandCharacteristics(string Name, string Value);
