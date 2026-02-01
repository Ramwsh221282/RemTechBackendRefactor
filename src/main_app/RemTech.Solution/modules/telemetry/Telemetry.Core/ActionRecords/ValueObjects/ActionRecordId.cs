using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Telemetry.Core.ActionRecords.ValueObjects;

/// <summary>
/// Идентификатор действия.
/// </summary>
public readonly record struct ActionRecordId
{
	/// <summary>
	/// Создает новый идентификатор действия.
	/// </summary>
	public ActionRecordId()
	{
		Value = Guid.NewGuid();
	}

	private ActionRecordId(Guid id)
	{
		Value = id;
	}

	/// <summary>
	/// Значение идентификатора действия.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создает новый идентификатор действия.
	/// </summary>
	/// <returns>Новый идентификатор действия.</returns>
	public static ActionRecordId New()
	{
		return new();
	}

	/// <summary>
	/// Создает новый идентификатор действия.
	/// </summary>
	/// <param name="id">Идентификатор действия.</param>
	/// <returns>Результат создания идентификатора действия.</returns>
	public static Result<ActionRecordId> Create(Guid id)
	{
		if (id == Guid.Empty)
		{
			return Error.Validation("Действие не может иметь пустой идентификатор.");
		}

		return new ActionRecordId(id);
	}
}
