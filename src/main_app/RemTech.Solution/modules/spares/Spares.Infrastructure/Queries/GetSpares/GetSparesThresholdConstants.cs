namespace Spares.Infrastructure.Queries.GetSpares;

public sealed class GetSparesThresholdConstants
{
	public double EmbeddingSearchThreshold { get; set; } = 0.4;
	public double TextSearchThreshold { get; set; } = 0.1;
	public double HybridSearchThreshold { get; set; } = 0.3;
}
