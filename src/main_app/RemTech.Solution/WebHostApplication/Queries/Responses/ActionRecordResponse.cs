namespace WebHostApplication.Queries.Responses;

/// <summary>
/// Результат запроса <see cref="GetActionRecordsQuery"/> .
/// </summary>
public sealed class ActionRecordResponse
{
	/// <summary>
	/// Идентификатор записи действия.
	/// </summary>
	public required Guid Id { get; set; }

	/// <summary>
	/// Идентификатор пользователя, выполнившего действие.
	/// </summary>
	public required Guid? UserId { get; set; }

	/// <summary>
	/// Идентификатор записи действия.
	/// </summary>
	public required string? UserLogin { get; set; }

	/// <summary>
	/// Email пользователя, выполнившего действие.
	/// </summary>
	public required string? UserEmail { get; set; }

	/// <summary>
	/// Разрешения пользователя, выполнившего действие.
	/// </summary>
	public required IReadOnlyList<ActionRecordUserPermissionResponse>? UserPermissions { get; set; }

	/// <summary>
	/// Имя действия.
	/// </summary>
	public required string ActionName { get; set; }

	/// <summary>
	/// Уровень серьезности действия.
	/// </summary>
	public required string ActionSeverity { get; set; }

	/// <summary>
	/// Сообщение об ошибке, если таковая имелась.
	/// </summary>
	public required string? ErrorMessage { get; set; }

	/// <summary>
	/// Временная метка действия.
	/// </summary>
	public required DateTime ActionTimestamp { get; set; }

	/// <summary>
	/// Общее количество записей, подходящих под фильтры запроса.
	/// </summary>
	public int TotalCount { get; set; }
}
