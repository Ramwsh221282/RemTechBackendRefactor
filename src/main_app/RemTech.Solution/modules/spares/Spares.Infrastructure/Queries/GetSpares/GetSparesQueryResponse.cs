namespace Spares.Infrastructure.Queries.GetSpares;

public sealed class GetSparesQueryResponse
{
	public int TotalCount { get; set; }
	public double AveragePrice { get; set; }
	public double MinimalPrice { get; set; }
	public double MaximalPrice { get; set; }
	public IReadOnlyCollection<SpareResponse> Spares { get; set; } = [];
}
