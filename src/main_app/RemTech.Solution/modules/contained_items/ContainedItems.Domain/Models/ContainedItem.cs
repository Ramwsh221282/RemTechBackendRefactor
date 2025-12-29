using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public sealed class ContainedItemStatus
{
    public static readonly ContainedItemStatus PendingToSave = new("PendingToSave");
    public static readonly ContainedItemStatus Saved = new("Saved");
    private static readonly ContainedItemStatus[] All = [PendingToSave, Saved];
    public string Value { get; }
    private ContainedItemStatus(string value) => Value = value;

    public static Result<ContainedItemStatus> CreateFromString(string value)
    {
        ContainedItemStatus? status = All.FirstOrDefault(s => s.Value == value);
        return status is null 
            ? Error.Validation($"Статус {value} невалиден.") 
            : status;
    }
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
    public ContainedItemStatus Status { get; private set; } = status;
    public static ContainedItem PendingToSave(
        ContainedItemId id, 
        ServiceItemId serviceItemId, 
        ServiceCreatorInfo creatorInfo, 
        ContainedItemInfo info) =>
        new(id, serviceItemId, creatorInfo, info, ContainedItemStatus.PendingToSave);

    public void MarkSaved()
    {
        Status = ContainedItemStatus.Saved;
    }
}