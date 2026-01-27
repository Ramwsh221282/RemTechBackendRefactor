namespace ContainedItems.Domain.Models;

/// <summary>
/// Модель содержащегося элемента.
/// </summary>
/// <param name="id">Идентификатор содержащегося элемента.</param>
/// <param name="serviceItemId">Идентификатор сервисного элемента.</param>
/// <param name="creatorInfo">Информация о создателе.</param>
/// <param name="info">Информация о содержащемся элементе.</param>
/// <param name="status">Статус содержащегося элемента.</param>
public sealed class ContainedItem(
	ContainedItemId id,
	ServiceItemId serviceItemId,
	ServiceCreatorInfo creatorInfo,
	ContainedItemInfo info,
	ContainedItemStatus status
)
{
	/// <summary>
	/// Идентификатор содержащегося элемента.
	/// </summary>
	public ContainedItemId Id { get; } = id;

	/// <summary>
	/// Идентификатор сервисного элемента.
	/// </summary>
	public ServiceItemId ServiceItemId { get; } = serviceItemId;

	/// <summary>
	/// Информация о создателе.
	/// </summary>
	public ServiceCreatorInfo CreatorInfo { get; } = creatorInfo;

	/// <summary>
	/// Информация о содержащемся элементе.
	/// </summary>
	public ContainedItemInfo Info { get; } = info;

	/// <summary>
	/// Статус содержащегося элемента.
	/// </summary>
	public ContainedItemStatus Status { get; private set; } = status;

	/// <summary>
	/// Создает новый содержащийся элемент с статусом "PendingToSave".
	/// </summary>
	/// <param name="id">Идентификатор содержащегося элемента.</param>
	/// <param name="serviceItemId">Идентификатор сервисного элемента.</param>
	/// <param name="creatorInfo">Информация о создателе.</param>
	/// <param name="info">Информация о содержащемся элементе.</param>
	/// <returns>Новый содержащийся элемент с статусом "PendingToSave".</returns>
	public static ContainedItem PendingToSave(
		ContainedItemId id,
		ServiceItemId serviceItemId,
		ServiceCreatorInfo creatorInfo,
		ContainedItemInfo info
	) => new(id, serviceItemId, creatorInfo, info, ContainedItemStatus.PendingToSave);

	/// <summary>
	/// Помечает содержащийся элемент как сохраненный, изменяя его статус на "Saved".
	/// </summary>
	public void MarkSaved() => Status = ContainedItemStatus.Saved;
}
