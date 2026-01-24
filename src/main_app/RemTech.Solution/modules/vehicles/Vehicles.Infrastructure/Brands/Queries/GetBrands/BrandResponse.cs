namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

// TODO: find out and fix why VehiclesCount, TotalCount and TextSearchScore are not initialized from cache.
public sealed class BrandResponse
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public float? TextSearchScore { get; set; }
	public int? VehiclesCount { get; set; }
	public int? TotalCount { get; set; }
}
