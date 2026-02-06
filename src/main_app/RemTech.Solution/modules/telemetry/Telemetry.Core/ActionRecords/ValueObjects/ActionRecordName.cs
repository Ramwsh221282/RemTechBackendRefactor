using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Telemetry.Core.ActionRecords.ValueObjects;

/// <summary>
/// Название действия.
/// </summary>
public sealed record ActionRecordName
{
	/// <summary>
	/// Максимальная длина названия действия.
	/// </summary>
	public const int MAX_LENGTH = 256;

	/// <summary>
	/// Создает новое название действия.
	/// </summary>
	/// <param name="value">Значение названия действия.</param>
	private ActionRecordName(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение названия действия.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает новое название действия.
	/// </summary>
	/// <param name="value">Значение названия действия.</param>
	/// <returns>Результат создания названия действия.</returns>
	public static Result<ActionRecordName> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return Error.Validation("Название действия не может быть пустым.");
		}

		if (value.Length > MAX_LENGTH)
		{
			return Error.Validation($"Название действия не может превышать {MAX_LENGTH} символов.");
		}

		return new ActionRecordName(value);
	}
}
