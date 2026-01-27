using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

/// <summary>
/// Статус содержащегося элемента.
/// </summary>
public sealed class ContainedItemStatus
{
	/// <summary>
	/// Статус, обозначающий, что элемент ожидает сохранения.
	/// </summary>
	public static readonly ContainedItemStatus PendingToSave = new("PendingToSave");

	/// <summary>
	/// Статус, обозначающий, что элемент был сохранен.
	/// </summary>
	public static readonly ContainedItemStatus Saved = new("Saved");

	private static readonly ContainedItemStatus[] _all = [PendingToSave, Saved];

	private ContainedItemStatus(string value) => Value = value;

	/// <summary>
	/// Значение статуса содержащегося элемента.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создает статус содержащегося элемента из строки.
	/// </summary>
	/// <param name="value">Строковое представление статуса содержащегося элемента.</param>
	/// <returns>Результат создания статуса содержащегося элемента.</returns>
	public static Result<ContainedItemStatus> CreateFromString(string value)
	{
		ContainedItemStatus? status = _all.FirstOrDefault(s => s.Value == value);
		return status is null ? Error.Validation($"Статус {value} невалиден.") : status;
	}
}
