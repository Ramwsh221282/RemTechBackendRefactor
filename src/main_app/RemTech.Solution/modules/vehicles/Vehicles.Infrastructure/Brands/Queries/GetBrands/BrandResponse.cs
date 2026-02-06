namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

/// <summary>
/// Ответ с информацией о бренде.
/// </summary>
public sealed class BrandResponse
{
	/// <summary>
	/// Идентификатор бренда.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Название бренда.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Оценка релевантности при текстовом поиске.
	/// </summary>
	public float? TextSearchScore { get; set; }

	/// <summary>
	/// Количество транспортных средств данного бренда.
	/// </summary>
	public int? VehiclesCount { get; set; }

	/// <summary>
	/// Общее количество брендов в ответе.
	/// </summary>
	public int? TotalCount { get; set; }
}
