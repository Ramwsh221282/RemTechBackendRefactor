using System.Text.Json.Serialization;

namespace WebHostApplication.Queries.GetActionRecords;

/// <summary>
/// Результат запроса <see cref="GetActionRecordsQuery"/> .
/// </summary>
public sealed class ActionRecordResponse
{
	/// <summary>
	/// Идентификатор записи действия.
	/// </summary>
	public Guid Id { get; init; }

	/// <summary>
	/// Идентификатор пользователя, выполнившего действие.
	/// </summary>
	public Guid? UserId { get; init; }

	/// <summary>
	/// Идентификатор записи действия.
	/// </summary>
	public required string? UserLogin { get; init; }

	/// <summary>
	/// Email пользователя, выполнившего действие.
	/// </summary>
	public required string? UserEmail { get; init; }

	/// <summary>
	/// Разрешения пользователя, выполнившего действие.
	/// </summary>
	public required IReadOnlyList<ActionRecordUserPermissionResponse>? UserPermissions { get; init; }

	/// <summary>
	/// Имя действия.
	/// </summary>
	public required string ActionName { get; init; }

	/// <summary>
	/// Уровень серьезности действия.
	/// </summary>
	public required string ActionSeverity { get; init; }

	/// <summary>
	/// Сообщение об ошибке, если таковая имелась.
	/// </summary>
	public required string? ErrorMessage { get; init; }

	/// <summary>
	/// Временная метка действия.
	/// </summary>
	public required DateTime ActionTimestamp { get; init; }

	/// <summary>
	/// Общее количество записей действий.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWriting)]
	public required int TotalCount { get; init; }
}
