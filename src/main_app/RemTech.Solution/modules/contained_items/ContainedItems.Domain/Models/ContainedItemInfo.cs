using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public sealed class ContainedItemInfo
{
	private ContainedItemInfo(string content, DateTime createdAt, DateTime? deletedAt)
	{
		Content = content;
		CreatedAt = createdAt;
		DeletedAt = deletedAt;
	}

	public static Result<ContainedItemInfo> Create(string content, DateTime createdAt, DateTime? deletedAt) =>
		string.IsNullOrWhiteSpace(content)
			? Error.Validation("Содержимое элемента не может быть пустым.")
			: new ContainedItemInfo(content, createdAt, deletedAt);

	public static Result<ContainedItemInfo> Create(string content) =>
		string.IsNullOrWhiteSpace(content)
			? Error.Validation("Содержимое элемента не может быть пустым.")
			: new ContainedItemInfo(content, createdAt: DateTime.UtcNow, deletedAt: null);

	public string Content { get; }
	public DateTime CreatedAt { get; }
	public DateTime? DeletedAt { get; }
}
