using Telemetry.Core.ActionRecords.ValueObjects;

namespace Telemetry.Core.ActionRecords;

/// <summary>
/// Запись действия.
/// </summary>
/// <param name="Id">Идентификатор записи действия.</param>
/// <param name="InvokerId">Идентификатор вызывающего объекта.</param>
/// <param name="Name">Имя действия.</param>
/// <param name="Severity">Уровень серьезности действия.</param>
/// <param name="PayloadJson">Данные действия в формате JSON.</param>
/// <param name="OccuredDateTime">Дата и время возникновения действия.</param>
public sealed record ActionRecord(
	ActionRecordId Id,
	ActionRecordInvokerId InvokerId,
	ActionRecordName Name,
	ActionRecordSeverity Severity,
	ActionRecordPayloadJson PayloadJson,
	ActionRecordOccuredDateTime OccuredDateTime
)
{
	private ActionRecord(
		ActionRecordInvokerId invokerId,
		ActionRecordName name,
		ActionRecordSeverity severity,
		ActionRecordPayloadJson payloadJson
	)
		: this(
			Id: ActionRecordId.New(),
			InvokerId: invokerId,
			Name: name,
			Severity: severity,
			PayloadJson: payloadJson,
			OccuredDateTime: ActionRecordOccuredDateTime.Now()
		) { }

	/// <summary>
	/// Создает успешный запись действия.
	/// </summary>
	/// <param name="invokerId">Идентификатор вызывающего объекта.</param>
	/// <param name="name">Имя действия.</param>
	/// <param name="payloadJson">Данные действия в формате JSON.</param>
	/// <returns>Новая запись действия с уровнем серьезности "успех".</returns>
	public static ActionRecord CreateSuccess(
		ActionRecordInvokerId invokerId,
		ActionRecordName name,
		ActionRecordPayloadJson payloadJson
	)
	{
		ActionRecordSeverity severity = ActionRecordSeverity.Success();
		return new ActionRecord(invokerId: invokerId, name: name, severity: severity, payloadJson: payloadJson);
	}

	/// <summary>
	/// Создает запись действия с уровнем серьезности "ошибка".
	/// </summary>
	/// <param name="invokerId">Идентификатор вызывающего объекта.</param>
	/// <param name="name">Имя действия.</param>
	/// <param name="payloadJson">Данные действия в формате JSON.</param>
	/// <returns>Новая запись действия с уровнем серьезности "ошибка".</returns>
	public static ActionRecord CreateError(
		ActionRecordInvokerId invokerId,
		ActionRecordName name,
		ActionRecordPayloadJson payloadJson
	)
	{
		ActionRecordSeverity severity = ActionRecordSeverity.Error();
		return new ActionRecord(invokerId: invokerId, name: name, severity: severity, payloadJson: payloadJson);
	}

	/// <summary>
	/// Создает запись действия с уровнем серьезности "предупреждение".
	/// </summary>
	/// <param name="invokerId">Идентификатор вызывающего объекта.</param>
	/// <param name="name">Имя действия.</param>
	/// <param name="payloadJson">Данные действия в формате JSON.</param>
	/// <returns>Новая запись действия с уровнем серьезности "предупреждение".</returns>
	public static ActionRecord CreateWarning(
		ActionRecordInvokerId invokerId,
		ActionRecordName name,
		ActionRecordPayloadJson payloadJson
	)
	{
		ActionRecordSeverity severity = ActionRecordSeverity.Warning();
		return new ActionRecord(invokerId: invokerId, name: name, severity: severity, payloadJson: payloadJson);
	}

	/// <summary>
	/// Создает информационную запись действия.
	/// </summary>
	/// <param name="invokerId">Идентификатор вызывающего объекта.</param>
	/// <param name="name">Имя действия.</param>
	/// <param name="payloadJson">Данные действия в формате JSON.</param>
	/// <returns>Новая запись действия с уровнем серьезности "информация".</returns>
	public static ActionRecord CreateInfo(
		ActionRecordInvokerId invokerId,
		ActionRecordName name,
		ActionRecordPayloadJson payloadJson
	)
	{
		ActionRecordSeverity severity = ActionRecordSeverity.Info();
		return new ActionRecord(invokerId: invokerId, name: name, severity: severity, payloadJson: payloadJson);
	}
}
