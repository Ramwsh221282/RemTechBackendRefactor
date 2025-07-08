using RemTech.ParsersManagement.External.ParsedItems.ParsedContainedItems.ValueObjects;

namespace RemTech.ParsersManagement.External.ParsedItems.ParsedContainedItems;

public sealed class ContainedItem
{
    private readonly ContainedItemId _id;
    private readonly ContainedItemSourceInfo _sourceInfo;

    public ContainedItem(ContainedItemId id, ContainedItemSourceInfo sourceInfo)
    {
        _id = id;
        _sourceInfo = sourceInfo;
    }
}
