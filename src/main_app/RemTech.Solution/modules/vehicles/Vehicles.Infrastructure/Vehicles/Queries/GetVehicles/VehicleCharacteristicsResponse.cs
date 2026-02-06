namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

/// <summary>
/// Характеристики транспортного средства в ответе на запрос.
/// </summary>
public sealed class VehicleCharacteristicsResponse
{
	/// <summary>
	/// Идентификатор характеристики.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Значение характеристики.
	/// </summary>
	public required string Value { get; set; }
}
