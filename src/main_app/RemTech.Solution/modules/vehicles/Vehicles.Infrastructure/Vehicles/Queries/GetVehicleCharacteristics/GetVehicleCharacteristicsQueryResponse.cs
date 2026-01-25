namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

public sealed class GetVehicleCharacteristicsQueryResponse
{
    public IReadOnlyCollection<VehicleCharacteristicsResponse> Characteristics { get; set; } = [];
}
