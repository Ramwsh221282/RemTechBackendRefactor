using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Telemetry.Core.ActionRecords.ValueObjects;

/// <summary>
/// Уровень серьезности действия.
/// </summary>
public sealed record ActionRecordSeverity
{
	/// <summary>
	/// Максимальная длина уровня серьезности действия.
	/// </summary>
	public const int MAX_LENGTH = 50;

	/// <summary>
	/// Успешное выполнение.
	/// </summary>
	public const string SUCCESS = "SUCCESS";

	/// <summary>
	/// Предупреждение.
	/// </summary>
	public const string WARNING = "WARNING";

	/// <summary>
	/// Ошибка.
	/// </summary>
	public const string ERROR = "ERROR";

	/// <summary>
	/// Информационное сообщение.
	/// </summary>
	public const string INFO = "INFO";

	/// <summary>
	/// Создает новый уровень серьезности действия.
	/// </summary>
	/// <param name="value">Значение уровня серьезности действия.</param>
	private ActionRecordSeverity(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение уровня серьезности действия.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает успешный уровень серьезности действия.
	/// </summary>
	/// <returns>Новый уровень серьезности действия.</returns>
	public static ActionRecordSeverity Success()
	{
		return new(SUCCESS);
	}

	/// <summary>
	/// Создает уровень серьезности действия с предупреждением.
	/// </summary>
	/// <returns>Новый уровень серьезности действия.</returns>
	public static ActionRecordSeverity Warning()
	{
		return new(WARNING);
	}

	/// <summary>
	/// Создает уровень серьезности действия с ошибкой.
	/// </summary>
	/// <returns>Новый уровень серьезности действия.</returns>
	public static ActionRecordSeverity Error()
	{
		return new(ERROR);
	}

	/// <summary>
	/// Создает информационный уровень серьезности действия.
	/// </summary>
	/// <returns>Новый уровень серьезности действия.</returns>
	public static ActionRecordSeverity Info()
	{
		return new(INFO);
	}

	/// <summary>
	/// Создает новый уровень серьезности действия.
	/// </summary>
	/// <param name="value">Значение уровня серьезности действия.</param>
	/// <returns>Результат создания уровня серьезности действия.</returns>
	public static Result<ActionRecordSeverity> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return RemTech.SharedKernel.Core.FunctionExtensionsModule.Error.Validation(
				"Уровень серьезности действия не может быть пустым."
			);
		}

		if (value.Length > MAX_LENGTH)
		{
			return RemTech.SharedKernel.Core.FunctionExtensionsModule.Error.Validation(
				$"Уровень серьезности действия не может превышать {MAX_LENGTH} символов."
			);
		}

		if (value is not SUCCESS and not WARNING and not ERROR and not INFO)
		{
			return RemTech.SharedKernel.Core.FunctionExtensionsModule.Error.Validation(
				$"Недопустимый уровень серьезности действия: {value}."
			);
		}

		return new ActionRecordSeverity(value);
	}
}
