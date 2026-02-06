namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

/// <summary>
/// Ответ на запрос получения транспортных средств по фильтрам.
/// </summary>
public sealed class GetVehiclesQueryResponse
{
	/// <summary>
	/// Общее количество транспортных средств.
	/// </summary>
	public int TotalCount { get; set; }

	/// <summary>
	/// Средняя цена транспортных средств.
	/// </summary>
	public double AveragePrice { get; set; }

	/// <summary>
	/// Минимальная и максимальная цена транспортных средств.
	/// </summary>
	public double MinimalPrice { get; set; }

	/// <summary>
	/// Максимальная цена транспортных средств.
	/// </summary>
	public double MaximalPrice { get; set; }

	/// <summary>
	/// Транспортные средства в ответе на запрос.
	/// </summary>
	public IReadOnlyCollection<VehicleResponse> Vehicles { get; set; } = [];
}
