namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed class VehicleResponse
{
	public required Guid VehicleId { get; set; }
	public required Guid BrandId { get; set; }
	public required string BrandName { get; set; }
	public required Guid CategoryId { get; set; }
	public required string CategoryName { get; set; }
	public required Guid ModelId { get; set; }
	public required string ModelName { get; set; }
	public required Guid RegionId { get; set; }
	public required string RegionName { get; set; }
	public required string Source { get; set; }
	public required long Price { get; set; }
	public required bool IsNds { get; set; }
	public required string Text { get; set; }
	public required int? ReleaseYear { get; set; }
	public required string[] Photos { get; set; }
	public required VehicleCharacteristicsResponse[] Characteristics { get; set; }
}
