using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Telemetry.Core.ActionRecords.ValueObjects;

/// <summary>
/// Идентификатор инициатора действия.
/// </summary>
public readonly record struct ActionRecordInvokerId
{
	/// <summary>
	/// Создает новый идентификатор инициатора действия.
	/// </summary>
	public ActionRecordInvokerId()
	{
		Value = Guid.NewGuid();
	}

	/// <summary>
	/// Создает новый идентификатор инициатора действия.
	/// </summary>
	/// <param name="value">Значение идентификатора инициатора действия.</param>
	private ActionRecordInvokerId(Guid value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение идентификатора инициатора действия.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создает новый идентификатор инициатора действия.
	/// </summary>
	/// <returns>Новый идентификатор инициатора действия.</returns>
	public static ActionRecordInvokerId New() => new();

	/// <summary>
	/// Создает новый идентификатор инициатора действия.
	/// </summary>
	/// <param name="id">Идентификатор инициатора действия.</param>
	/// <returns>Результат создания идентификатора инициатора действия.</returns>
	public static Result<ActionRecordInvokerId> Create(Guid id)
	{
		if (id == Guid.Empty)
		{
			return Error.Validation("Идентификатор инициатора действия не может быть пустым.");
		}

		return new ActionRecordInvokerId(id);
	}
}
