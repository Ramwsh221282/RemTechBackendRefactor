using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Telemetry.Core.ActionRecords.ValueObjects;

/// <summary>
/// Ошибка записи действия пользователя.
/// </summary>
public sealed record ActionRecordError
{
	private ActionRecordError(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение ошибки действия пользователя.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создать ошибку записи действия пользователя.
	/// </summary>
	/// <param name="value">Значение ошибки действия пользователя.</param>
	/// <returns>Результат создания ошибки действия пользователя.</returns>
	public static Result<ActionRecordError> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Ошибка действия пользователя не может быть пустой.");
		return Result.Success(new ActionRecordError(value));
	}

	/// <summary>
	/// Пробует создать ошибку записи действия пользователя из nullable строки.
	/// </summary>
	/// <param name="value">Значение ошибки действия пользователя.</param>
	/// <returns>Ошибка записи действия пользователя или null, если значение пустое.</returns>
	public static ActionRecordError? FromNullableString(string? value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return null;
		return new ActionRecordError(value);
	}
}
