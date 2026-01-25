namespace ContainedItems.Domain.Models;

public sealed class ContainedItem(
	ContainedItemId id,
	ServiceItemId serviceItemId,
	ServiceCreatorInfo creatorInfo,
	ContainedItemInfo info,
	ContainedItemStatus status
)
{
	public ContainedItemId Id { get; } = id;
	public ServiceItemId ServiceItemId { get; } = serviceItemId;
	public ServiceCreatorInfo CreatorInfo { get; } = creatorInfo;
	public ContainedItemInfo Info { get; } = info;
	public ContainedItemStatus Status { get; private set; } = status;

	public static ContainedItem PendingToSave(
		ContainedItemId id,
		ServiceItemId serviceItemId,
		ServiceCreatorInfo creatorInfo,
		ContainedItemInfo info
	) => new(id, serviceItemId, creatorInfo, info, ContainedItemStatus.PendingToSave);

	public void MarkSaved() => Status = ContainedItemStatus.Saved;
}
