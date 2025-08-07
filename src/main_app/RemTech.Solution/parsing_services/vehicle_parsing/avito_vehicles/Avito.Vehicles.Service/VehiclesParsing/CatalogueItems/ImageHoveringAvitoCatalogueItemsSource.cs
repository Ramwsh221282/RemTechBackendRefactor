using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.CatalogueItems;

public sealed class ImageHoveringAvitoCatalogueItemsSource(
    IPage page,
    IAvitoCatalogueItemsSource origin
) : IAvitoCatalogueItemsSource
{
    private readonly string _containerSelector = string.Intern("#bx_serp-item-list");
    private readonly string _itemSelector = string.Intern("div[data-marker='item']");
    private readonly string _sliderSelector = string.Intern(".iva-item-slider-BOsti");
    private readonly string _photoSelector = string.Intern(".photo-slider-list-R0jle");

    public async Task<CatalogueItemsList> Read()
    {
        CatalogueItemsList fromOrigin = await origin.Read();
        IElementHandle container = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_containerSelector);
        IElementHandle[] items = await new ParentManyElementsSource(container).Read(_itemSelector);
        foreach (var item in items)
        {
            IElementHandle? slider = await new ParentElementSource(item).Read(_sliderSelector);
            if (slider == null)
                continue;
            IElementHandle sliderList = await new ParentElementSource(slider).Read(_photoSelector);
            if (sliderList == null)
                continue;
            await sliderList.FocusAsync();
            await sliderList.HoverAsync();
        }

        return fromOrigin;
    }
}
