using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Telemetry.Core.ActionRecords.ValueObjects;

/// <summary>
/// Полезная нагрузка действия в формате JSON.
/// </summary>
public sealed record ActionRecordPayloadJson
{
	private ActionRecordPayloadJson(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение полезной нагрузки действия в формате JSON.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает полезную нагрузку действия в формате JSON.
	/// </summary>
	/// <param name="value">Значение полезной нагрузки действия в формате JSON.</param>
	/// <returns>Результат создания полезной нагрузки действия в формате JSON.</returns>
	public static Result<ActionRecordPayloadJson> Create(string value) => new ActionRecordPayloadJson(value);
}
