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
        try
        {
            await Hover();
        }
        catch
        {
            await Hover();
        }
        return fromOrigin;
    }

    private async Task Hover(int repeatTimes = 5)
    {
        for (int i = 0; i < repeatTimes; i++)
        {
            try
            {
                IElementHandle container = await new ValidSingleElementSource(
                    new PageElementSource(page)
                ).Read(_containerSelector);
                IElementHandle[] items = await new ParentManyElementsSource(container).Read(
                    _itemSelector
                );
                foreach (var item in items)
                {
                    try
                    {
                        await HoverImageElement(item);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                break;
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }

    private async Task HoverImageElement(IElementHandle item)
    {
        IElementHandle slider = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(_sliderSelector);
        IElementHandle sliderList = await new ValidSingleElementSource(
            new ParentElementSource(slider)
        ).Read(_photoSelector);
        await sliderList.FocusAsync();
        await sliderList.HoverAsync();
    }
}
