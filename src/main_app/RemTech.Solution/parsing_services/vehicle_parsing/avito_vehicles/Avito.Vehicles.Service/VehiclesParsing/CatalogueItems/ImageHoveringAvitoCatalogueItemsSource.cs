using Parsing.Avito.Common.Images;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;

public sealed class ImageHoveringAvitoCatalogueItemsSource(
    IPage page,
    IAvitoCatalogueItemsSource origin
) : IAvitoCatalogueItemsSource
{
    public async Task<CatalogueItemsList> Read()
    {
        CatalogueItemsList fromOrigin = await origin.Read();
        await new AvitoImagesHoverer(page).HoverImages();
        return fromOrigin;
    }
}
