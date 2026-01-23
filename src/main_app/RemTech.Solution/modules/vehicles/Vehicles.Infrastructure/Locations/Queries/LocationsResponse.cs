namespace Vehicles.Infrastructure.Locations.Queries;

public sealed class LocationsResponse(Guid id, string name)
{
	public Guid Id { get; } = id;
	public string Name { get; } = name;
	public float? TextSearchScore { get; set; }
	public int? VehiclesCount { get; set; }
}
