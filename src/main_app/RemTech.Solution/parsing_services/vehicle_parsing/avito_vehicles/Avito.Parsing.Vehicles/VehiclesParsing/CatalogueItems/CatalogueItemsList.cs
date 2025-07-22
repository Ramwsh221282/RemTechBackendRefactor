namespace Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;

public sealed class CatalogueItemsList
{
    private readonly LinkedList<CatalogueItem> _items = [];

    public CatalogueItemsList With(CatalogueItem item)
    {
        _items.AddFirst(item);
        return this;
    }

    public IEnumerable<CatalogueItem> Iterate()
    {
        foreach (var item in _items)
        {
            yield return item;
        }
    }

    public int Amount() => _items.Count;
}