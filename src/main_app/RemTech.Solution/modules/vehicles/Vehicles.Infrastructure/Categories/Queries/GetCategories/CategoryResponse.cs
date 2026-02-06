namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

/// <summary>
/// Ответ с информацией о категории транспортных средств.
/// </summary>
public sealed class CategoryResponse
{
	/// <summary>
	/// Идентификатор категории транспортных средств.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Название категории транспортных средств.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Количество транспортных средств в категории.
	/// </summary>
	public int? VehiclesCount { get; set; }

	/// <summary>
	/// Оценка релевантности при текстовом поиске.
	/// </summary>
	public float? TextSearchScore { get; set; }

	/// <summary>
	/// Общее количество категорий в ответе.
	/// </summary>
	public int? TotalCategoriesCount { get; set; }
}
