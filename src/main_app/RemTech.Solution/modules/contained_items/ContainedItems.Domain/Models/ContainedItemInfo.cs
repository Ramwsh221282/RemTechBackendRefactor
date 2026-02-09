using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

/// <summary>
/// Информация о содержащемся элементе.
/// </summary>
public sealed class ContainedItemInfo
{
	private ContainedItemInfo(string content, DateTime createdAt, DateTime? deletedAt)
	{
		Content = content;
		CreatedAt = createdAt;
		DeletedAt = deletedAt;
	}

	/// <summary>
	/// Содержимое элемента.
	/// </summary>
	public string Content { get; }

	/// <summary>
	/// Дата и время создания элемента.
	/// </summary>
	public DateTime CreatedAt { get; }

	/// <summary>
	/// Дата и время удаления элемента, если он был удален.
	/// </summary>
	public DateTime? DeletedAt { get; }

	/// <summary>
	/// Создает информацию о содержащемся элементе.
	/// </summary>
	/// <param name="content">Содержимое элемента.</param>
	/// <param name="createdAt">Дата и время создания элемента.</param>
	/// <param name="deletedAt">Дата и время удаления элемента, если он был удален.</param>
	/// <returns>Результат создания информации о содержащемся элементе.</returns>
	public static Result<ContainedItemInfo> Create(string content, DateTime createdAt, DateTime? deletedAt)
	{
		return string.IsNullOrWhiteSpace(content)
			? Error.Validation("Содержимое элемента не может быть пустым.")
			: new ContainedItemInfo(content, createdAt, deletedAt);
	}

	/// <summary>
	/// Создает информацию о содержащемся элементе.
	/// </summary>
	/// <param name="content">Содержимое элемента.</param>
	/// <returns>Результат создания информации о содержащемся элементе.</returns>
	public static Result<ContainedItemInfo> Create(string content)
	{
		return string.IsNullOrWhiteSpace(content)
			? Error.Validation("Содержимое элемента не может быть пустым.")
			: new ContainedItemInfo(content, createdAt: DateTime.UtcNow, deletedAt: null);
	}
}
