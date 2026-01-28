namespace Vehicles.Infrastructure.Locations.Queries;

/// <summary>
/// Ответ с информацией о локации.
/// </summary>
public sealed class LocationsResponse
{
	/// <summary>
	/// Идентификатор локации.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Название локации.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Описание локации.
	/// </summary>
	public float? TextSearchScore { get; set; }

	/// <summary>
	/// Количество транспортных средств в локации.
	/// </summary>
	public int? VehiclesCount { get; set; }
}
