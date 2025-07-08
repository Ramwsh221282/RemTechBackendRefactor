using RemTech.ParsersManagement.External.ParsedItems.ParsedContainedItems;
using RemTech.ParsersManagement.External.ParsedItems.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.External.ParsedItems;

public sealed class ContainedItemProducer
{
    private readonly ContainedItemProducerIdentity _identity;
    private ContainedItem[] _containedItems;

    public ContainedItemProducer(
        ContainedItemProducerIdentity identity,
        ContainedItem[] containedItems
    )
    {
        _identity = identity;
        _containedItems = containedItems;
    }

    public ContainedItemProducer(ContainedItemProducerIdentity identity)
        : this(identity, []) { }

    public Status<ContainedItem> Contained(ContainedItem item)
    {
        _containedItems = [.. _containedItems, item];
        return item;
    }

    public Status<TContainedItem> Contain<TContainedItem>(Func<Status<TContainedItem>> containFn)
    {
        TContainedItem containedItem = containFn();
        return containedItem;
    }
}
