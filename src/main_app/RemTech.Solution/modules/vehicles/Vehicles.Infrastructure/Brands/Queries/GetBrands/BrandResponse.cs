namespace Vehicles.Infrastructure.Brands.Queries.GetBrands;

public sealed class BrandResponse
{
	public Guid Id { get; set; }
	public required string Name { get; set; }
	public float? TextSearchScore { get; set; }
	public int? VehiclesCount { get; set; }
	public int? TotalCount { get; set; }
}
