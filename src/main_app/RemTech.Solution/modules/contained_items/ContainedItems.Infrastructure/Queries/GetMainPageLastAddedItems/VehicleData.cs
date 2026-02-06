namespace ContainedItems.Infrastructure.Queries.GetMainPageLastAddedItems;

/// <summary>
/// Данные техники.
/// </summary>
/// <param name="Id">Идентификатор техники.</param>
/// <param name="Title">Название техники.</param>
/// <param name="Photos">Фотографии техники.</param>
/// <param name="Characteristics">Характеристики техники.</param>
public sealed record VehicleData(
	Guid Id,
	string Title,
	IEnumerable<string> Photos,
	IEnumerable<VehicleDataCharacteristic> Characteristics
);
