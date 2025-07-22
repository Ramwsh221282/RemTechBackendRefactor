using Parsing.SDK.ScrapingActions;

namespace Avito.Parsing.Vehicles.VehiclesParsing.CatalogueItems;

public sealed class NavigatingCatalogueItems(
    IPageNavigating navigating, 
    IAvitoCatalogueItemsSource origin)
    : IAvitoCatalogueItemsSource
{
    public async Task<CatalogueItemsList> Read()
    {
        await navigating.Do();
        return await origin.Read();
    }
}