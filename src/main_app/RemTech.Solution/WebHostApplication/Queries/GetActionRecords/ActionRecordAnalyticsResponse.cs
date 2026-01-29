namespace WebHostApplication.Queries.GetActionRecords;

/// <summary>
/// Ответ с записью действия.
/// </summary>
public sealed class ActionRecordAnalyticsResponse
{
	/// <summary>
	/// Дата по дням.
	/// </summary>
	public required DateTime DateByDay { get; set; }

	/// <summary>
	/// Элементы записи действий.
	/// </summary>
	public IReadOnlyList<ActionRecordResponse> Results { get; set; } = [];
}
