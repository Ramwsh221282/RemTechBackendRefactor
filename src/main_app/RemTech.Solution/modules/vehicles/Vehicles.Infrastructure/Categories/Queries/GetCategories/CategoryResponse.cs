namespace Vehicles.Infrastructure.Categories.Queries.GetCategories;

// TODO: find out and fix why VehiclesCount and TextSearchScore are not initialized from cache.
public sealed class CategoryResponse
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public int? VehiclesCount { get; set; }
	public float? TextSearchScore { get; set; }
}
