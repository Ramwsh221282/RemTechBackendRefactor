namespace Vehicles.Infrastructure.Locations.Queries;

public sealed class LocationsResponse
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public float? TextSearchScore { get; set; }
	public int? VehiclesCount { get; set; }
}
