using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Идентификатор вложенного элемента (запчасти).
/// </summary>
public readonly record struct ContainedItemId
{
	/// <summary>
	/// Создаёт новый идентификатор.
	/// </summary>
	public ContainedItemId()
	{
		Value = Guid.NewGuid();
	}

	private ContainedItemId(Guid value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение идентификатора.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создаёт идентификатор из заданного значения.
	/// </summary>
	/// <param name="value">Значение идентификатора.</param>
	/// <returns>Результат создания идентификатора.</returns>
	public static Result<ContainedItemId> Create(Guid value) =>
		value == Guid.Empty
			? Error.Validation("Идентификатор запчасти не может быть пустым.")
			: Result.Success(new ContainedItemId(value));
}
