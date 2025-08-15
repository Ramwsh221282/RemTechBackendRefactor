using Cleaners.Module.RabbitMq;

namespace Cleaners.Module.Domain;

internal sealed class CleanerItemsCollection(IEnumerable<CleanerItem> items)
{
    private readonly List<CleanerItem> _items = [.. items];

    public void PrintTo(StartCleaningMessage message)
    {
        foreach (var item in _items)
            item.PrintTo(message);
    }

    public bool HasAnything() => _items.Count > 0;
}
