using System.Diagnostics;
using ContainedItems.Infrastructure.Producers;

namespace ContainedItems.Infrastructure.BackgroundServices;

public sealed class ItemPublishStrategyFactory(AddSparesProducer addSpares)
{
    private AddSparesProducer Spares { get; } = addSpares;

    public IItemPublishingStrategy Resolve(string itemType)
    {
        return itemType switch
        {
            "Запчасти" => Spares,
            _ => throw new UnreachableException("Unknown item type: " + itemType)
        };
    }
}