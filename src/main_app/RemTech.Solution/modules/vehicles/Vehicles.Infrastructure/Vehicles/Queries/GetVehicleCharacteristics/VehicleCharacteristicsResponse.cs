namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

public sealed class VehicleCharacteristicsResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string[] Values { get; set; }
}
