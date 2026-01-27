using Telemetry.Core.ActionRecords.ValueObjects;

namespace Telemetry.Core.ActionRecords;

/// <summary>
/// Запись действия пользователя.
/// </summary>
public sealed class ActionRecord
{
	/// <summary>
	/// Создает новую запись действия пользователя.
	/// </summary>
	public required ActionRecordId Id { get; init; }

	/// <summary>
	/// Идентификатор инициатора действия.
	/// </summary>
	public required ActionRecordInvokerId? InvokerId { get; init; }

	/// <summary>
	/// Название действия.
	/// </summary>
	public required ActionRecordName Name { get; init; }

	/// <summary>
	/// Уровень серьезности действия.
	/// </summary>
	public required ActionRecordSeverity Severity { get; init; }

	/// <summary>
	/// Полезная нагрузка действия в формате JSON.
	/// </summary>
	public required ActionRecordPayloadJson? PayloadJson { get; init; }

	/// <summary>
	/// Дата и время совершения действия.
	/// </summary>
	public required ActionRecordOccuredDateTime OccuredDateTime { get; init; }

	/// <summary>
	/// Ошибка действия пользователя.
	/// </summary>
	public required ActionRecordError? Error { get; init; }
}
