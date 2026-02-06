namespace Spares.Infrastructure.Queries.GetSparesLocations;

/// <summary>
/// Ответ с информацией о локации запчасти.
/// </summary>
public sealed class SpareLocationResponse
{
	/// <summary>
	/// Идентификатор локации запчасти.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Название локации запчасти.
	/// </summary>
	public required string Name { get; set; }
}
