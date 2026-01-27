using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

/// <summary>
/// Идентификатор содержащегося элемента.
/// </summary>
public readonly record struct ContainedItemId
{
	/// <summary>
	/// Инициализирует новый экземпляр <see cref="ContainedItemId"/> с новым уникальным значением.
	/// </summary>
	public ContainedItemId() => Value = Guid.NewGuid();

	private ContainedItemId(Guid value) => Value = value;

	/// <summary>
	/// Значение идентификатора содержащегося элемента.
	/// </summary>
	public Guid Value { get; }

	/// <summary>
	/// Создает новый идентификатор содержащегося элемента.
	/// </summary>
	/// <returns>Новый идентификатор содержащегося элемента.</returns>
	public static ContainedItemId New() => new();

	/// <summary>
	/// Создает идентификатор содержащегося элемента из заданного значения.
	/// </summary>
	/// <param name="value">Значение идентификатора содержащегося элемента.</param>
	/// <returns>Результат создания идентификатора содержащегося элемента.</returns>
	public static Result<ContainedItemId> Create(Guid value) =>
		value == Guid.Empty
			? Error.Validation("Идентификатор сохраняемого элемента не может быть пустым.")
			: new ContainedItemId(value);
}
