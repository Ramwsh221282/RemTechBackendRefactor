namespace ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;

/// <summary>
/// Характеристика техники.
/// </summary>
/// <param name="Characteristic">Название характеристики.</param>
/// <param name="Value">Значение характеристики.</param>
public sealed record VehicleDataCharacteristic(string Characteristic, string Value);
