namespace Spares.Infrastructure.Queries.GetSpares;

/// <summary>
/// Ответ на запрос получения запчастей.
/// </summary>
public sealed class GetSparesQueryResponse
{
	/// <summary>
	/// Общее количество запчастей, соответствующих запросу.
	/// </summary>
	public int TotalCount { get; set; }

	/// <summary>
	/// Средняя цена запчастей, соответствующих запросу.
	/// </summary>
	public double AveragePrice { get; set; }

	/// <summary>
	/// Минимальная цена запчастей, соответствующих запросу.
	/// </summary>
	public double MinimalPrice { get; set; }

	/// <summary>
	/// Максимальная цена запчастей, соответствующих запросу.
	/// </summary>
	public double MaximalPrice { get; set; }

	/// <summary>
	/// 	Коллекция запчастей, соответствующих запросу.
	/// </summary>
	public IReadOnlyCollection<SpareResponse> Spares { get; set; } = [];
}
