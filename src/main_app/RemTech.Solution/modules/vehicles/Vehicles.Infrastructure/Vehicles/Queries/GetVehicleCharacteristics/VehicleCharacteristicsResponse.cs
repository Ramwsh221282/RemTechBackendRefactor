namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicleCharacteristics;

/// <summary>
/// Характеристики транспортного средства в ответе на запрос.
/// </summary>
public sealed class VehicleCharacteristicsResponse
{
	/// <summary>
	/// Идентификатор характеристики.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Название характеристики.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Значения характеристики.
	/// </summary>
	public required IReadOnlyList<string> Values { get; set; }
}
