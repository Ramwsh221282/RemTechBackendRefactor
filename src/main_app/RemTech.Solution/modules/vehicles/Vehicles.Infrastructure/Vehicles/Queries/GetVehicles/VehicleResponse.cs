namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

/// <summary>
/// Транспортное средство в ответе на запрос.
/// </summary>
public sealed class VehicleResponse
{
	/// <summary>
	/// Идентификатор транспортного средства.
	/// </summary>
	public required Guid VehicleId { get; set; }

	/// <summary>
	/// Идентификатор и название бренда транспортного средства.
	/// </summary>
	public required Guid BrandId { get; set; }

	/// <summary>
	/// Название бренда транспортного средства.
	/// </summary>
	public required string BrandName { get; set; }

	/// <summary>
	/// Идентификатор и название категории транспортного средства.
	/// </summary>
	public required Guid CategoryId { get; set; }

	/// <summary>
	/// Название категории транспортного средства.
	/// </summary>
	public required string CategoryName { get; set; }

	/// <summary>
	/// Идентификатор и название модели транспортного средства.
	/// </summary>
	public required Guid ModelId { get; set; }

	/// <summary>
	/// Название модели транспортного средства.
	/// </summary>
	public required string ModelName { get; set; }

	/// <summary>
	/// Идентификатор и название региона транспортного средства.
	/// </summary>
	public required Guid RegionId { get; set; }

	/// <summary>
	/// Название региона транспортного средства.
	/// </summary>
	public required string RegionName { get; set; }

	/// <summary>
	/// Источник информации о транспортном средстве.
	/// </summary>
	public required string Source { get; set; }

	/// <summary>
	/// Цена транспортного средства.
	/// </summary>
	public required long Price { get; set; }

	/// <summary>
	/// Флаг НДС.
	/// </summary>
	public required bool IsNds { get; set; }

	/// <summary>
	/// Описание транспортного средства.
	/// </summary>
	public required string Text { get; set; }

	/// <summary>
	/// Год выпуска транспортного средства.
	/// </summary>
	public required int? ReleaseYear { get; set; }

	/// <summary>
	/// Фотографии транспортного средства.
	/// </summary>
	public required IReadOnlyList<string> Photos { get; set; }

	/// <summary>
	/// Характеристики транспортного средства.
	/// </summary>
	public required IReadOnlyList<VehicleCharacteristicsResponse> Characteristics { get; set; }
}
