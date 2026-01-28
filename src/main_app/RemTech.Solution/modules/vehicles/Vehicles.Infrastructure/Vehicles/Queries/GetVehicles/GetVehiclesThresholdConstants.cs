namespace Vehicles.Infrastructure.Vehicles.Queries.GetVehicles;

/// <summary>
/// Константы порогов для поиска транспортных средств.
/// </summary>
public sealed class GetVehiclesThresholdConstants
{
	/// <summary>
	/// Порог для векторного поиска.
	/// </summary>
	public double EmbeddingSearchThreshold { get; set; } = 0.4;

	/// <summary>
	/// Порог для текстового поиска.
	/// </summary>
	public double TextSearchThreshold { get; set; } = 0.1;

	/// <summary>
	/// Порог для гибридного поиска.
	/// </summary>
	public double HybridSearchThreshold { get; set; } = 0.3;
}
