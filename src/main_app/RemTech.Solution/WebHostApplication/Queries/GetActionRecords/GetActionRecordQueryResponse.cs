namespace WebHostApplication.Queries.GetActionRecords;

/// <summary>
/// Ответ на запрос <see cref="GetActionRecordsQuery"/> .
/// </summary>
public sealed class GetActionRecordQueryResponse
{
	/// <summary>
	/// Общее количество записей действий.
	/// </summary>
	public int TotalCount { get; set; }

	/// <summary>
	/// Максимальное количество страниц.
	/// </summary>
	public int MaxPage { get; set; }

	/// <summary>
	/// Количество страниц.
	/// </summary>
	public int PagesCount { get; set; }

	/// <summary>
	/// Номер текущей страницы.
	/// </summary>
	public int PageNumber { get; set; }

	/// <summary>
	/// Размер страницы.
	/// </summary>
	public int PageSize { get; set; }

	/// <summary>
	/// Есть ли следующая страница.
	/// </summary>
	public bool HasNextPage { get; set; }

	/// <summary>
	/// Есть ли предыдущая страница.
	/// </summary>
	public bool HasPreviousPage { get; set; }

	/// <summary>
	/// Элементы записи действий.
	/// </summary>
	public IReadOnlyList<ActionRecordAnalyticsResponse> Items { get; set; } = [];
}

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
