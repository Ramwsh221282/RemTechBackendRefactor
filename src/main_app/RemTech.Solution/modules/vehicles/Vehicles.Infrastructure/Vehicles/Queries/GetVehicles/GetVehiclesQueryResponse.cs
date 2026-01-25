namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

public sealed class GetVehiclesQueryResponse
{
	public int TotalCount { get; set; }
	public double AveragePrice { get; set; }
	public double MinimalPrice { get; set; }
	public double MaximalPrice { get; set; }
	public IReadOnlyCollection<VehicleResponse> Vehicles { get; set; } = [];
}
