namespace Spares.Infrastructure.Queries.GetSpares;

/// <summary>
/// Константы пороговых значений для текстового поиска запчастей.
/// </summary>
public sealed class GetSparesThresholdConstants
{
	/// <summary>
	/// Пороговое значение для поиска с использованием эмбеддингов.
	/// </summary>
	public double EmbeddingSearchThreshold { get; set; } = 0.4;

	/// <summary>
	/// Пороговое значение для текстового поиска.
	/// </summary>
	public double TextSearchThreshold { get; set; } = 0.1;

	/// <summary>
	/// Пороговое значение для гибридного поиска.
	/// </summary>
	public double HybridSearchThreshold { get; set; } = 0.3;
}
