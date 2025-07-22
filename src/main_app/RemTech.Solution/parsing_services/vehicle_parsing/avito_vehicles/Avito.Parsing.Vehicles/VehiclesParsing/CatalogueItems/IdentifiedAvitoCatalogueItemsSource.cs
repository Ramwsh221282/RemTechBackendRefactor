namespace Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;

public sealed class IdentifiedAvitoCatalogueItemsSource(
    IAvitoCatalogueItemsSource origin)
    : IAvitoCatalogueItemsSource
{
    public async Task<CatalogueItemsList> Read()
    {
        CatalogueItemsList fromOrigin = await origin.Read();
        CatalogueItemsList newList = new CatalogueItemsList();
        foreach (CatalogueItem item in fromOrigin.Iterate())
            newList = newList.With(item.Identified());
        return newList;
    }
}