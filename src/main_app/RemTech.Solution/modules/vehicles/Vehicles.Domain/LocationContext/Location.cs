using Vehicles.Domain.LocationContext.ValueObjects;

namespace Vehicles.Domain.LocationContext;

public sealed class Location
{
    public required LocationId Id { get; set; }
    public required LocationAddress Address { get; set; }
    public required LocationRating Rating { get; set; }
    public required LocationVehiclesCount VehicleCount { get; set; }
}
