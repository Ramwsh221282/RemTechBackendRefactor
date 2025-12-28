namespace ContainedItems.Domain.Models;

public sealed class ContainedItemStatus
{
    public static readonly ContainedItemStatus PendingToSave = new("PendingToSave");
    public static readonly ContainedItemStatus Saved = new("Saved");
    public string Value { get; }
    private ContainedItemStatus(string value) => Value = value;
}

public sealed class ContainedItem(ContainedItemId id, 
    ServiceItemId serviceItemId, 
    ServiceCreatorInfo creatorInfo, 
    ContainedItemInfo info, 
    ContainedItemStatus status)
{
    public ContainedItemId Id { get; } = id;
    public ServiceItemId ServiceItemId { get; } = serviceItemId;
    public ServiceCreatorInfo CreatorInfo { get; } = creatorInfo;
    public ContainedItemInfo Info { get; } = info;
    public ContainedItemStatus Status { get; } = status;
    public static ContainedItem PendingToSave(
        ContainedItemId id, 
        ServiceItemId serviceItemId, 
        ServiceCreatorInfo creatorInfo, 
        ContainedItemInfo info) =>
        new(id, serviceItemId, creatorInfo, info, ContainedItemStatus.PendingToSave);
}