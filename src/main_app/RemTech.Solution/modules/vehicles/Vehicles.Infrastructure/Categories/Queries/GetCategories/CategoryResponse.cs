namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

public sealed class CategoryResponse
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public int? VehiclesCount { get; set; }
	public float? TextSearchScore { get; set; }
	public int? TotalCategoriesCount { get; set; }
}
