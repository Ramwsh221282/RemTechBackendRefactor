using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;

public sealed class NavigatingCatalogueItems(
    IPage page,
    IPageNavigating navigating,
    IAvitoCatalogueItemsSource origin
) : IAvitoCatalogueItemsSource
{
    public async Task<CatalogueItemsList> Read()
    {
        await navigating.Do();
        await new PageBottomScrollingAction(page).Do();
        await new PageUpperScrollingAction(page).Do();
        return await origin.Read();
    }
}
