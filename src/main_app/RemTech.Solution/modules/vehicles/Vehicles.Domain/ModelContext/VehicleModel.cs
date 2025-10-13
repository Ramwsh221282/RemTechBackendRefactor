using Vehicles.Domain.ModelContext.ValueObjects;

namespace Vehicles.Domain.ModelContext;

public sealed class VehicleModel
{
    public required VehicleModelId Id { get; set; }
    public required VehicleModelName Name { get; set; }
    public required VehicleModelRating Rating { get; set; }
    public required VehicleModelVehicleCount VehiclesCount { get; set; }
}
