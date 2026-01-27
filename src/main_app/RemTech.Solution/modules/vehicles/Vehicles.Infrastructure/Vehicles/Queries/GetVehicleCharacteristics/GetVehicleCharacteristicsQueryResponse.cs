namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

/// <summary>
/// Ответ на запрос получения характеристик транспортных средств по фильтрам.
/// </summary>
public sealed class GetVehicleCharacteristicsQueryResponse
{
	/// <summary>
	/// Характеристики транспортных средств.
	/// </summary>
	public IReadOnlyCollection<VehicleCharacteristicsResponse> Characteristics { get; set; } = [];
}
