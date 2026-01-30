namespace WebHostApplication.Queries.GetActionRecordsStatistics;

/// <summary>
///  Ответ с статистикой записей действий.
/// </summary>
public sealed class ActionRecordsStatisticsResponse
{
	/// <summary>
	/// Дата записи.
	/// </summary>
	public required DateTime Date { get; set; }

	/// <summary>
	/// Количество записей.
	/// </summary>
	public required int Amount { get; set; }
}
