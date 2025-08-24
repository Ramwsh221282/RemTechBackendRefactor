using Parsing.Grpc.Services.DuplicateIds;

namespace Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;

public sealed class CatalogueItemsList
{
    private LinkedList<CatalogueItem> _items = [];

    public CatalogueItemsList With(CatalogueItem item)
    {
        _items.AddFirst(item);
        return this;
    }

    public async Task Filter(GrpcDuplicateIdsClient client)
    {
        IEnumerable<string> duplicateIds = await client.GetDuplicateIdentifiers(
            _items.Select(i => i.ReadId().StringValue())
        );
        HashSet<string> dupSet = new HashSet<string>(duplicateIds, StringComparer.Ordinal);
        _items = new LinkedList<CatalogueItem>(
            _items
                .DistinctBy(s => s.ReadId().StringValue())
                .Where(s => !dupSet.Contains(s.ReadId().StringValue()))
        );
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
