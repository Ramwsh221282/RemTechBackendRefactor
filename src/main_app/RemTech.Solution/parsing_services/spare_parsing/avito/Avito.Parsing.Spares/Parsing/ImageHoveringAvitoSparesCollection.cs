using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Avito.Parsing.Spares.Parsing;

public sealed class ImageHoveringAvitoSparesCollection(IPage page, IAvitoSparesCollection origin)
    : IAvitoSparesCollection
{
    private const string ContainerSelector = "#bx_serp-item-list";
    private const string ItemSelector = "div[data-marker='item']";
    private const string SliderSelector = ".iva-item-slider-BOsti";
    private const string PhotoSelector = ".photo-slider-list-R0jle";

    public async Task<IEnumerable<AvitoSpare>> Read()
    {
        IElementHandle container = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(ContainerSelector);
        IElementHandle[] items = await new ParentManyElementsSource(container).Read(ItemSelector);
        foreach (var item in items)
        {
            try
            {
                IElementHandle slider = await new ValidSingleElementSource(
                    new ParentElementSource(item)
                ).Read(SliderSelector);
                IElementHandle sliderList = await new ValidSingleElementSource(
                    new ParentElementSource(slider)
                ).Read(PhotoSelector);
                await sliderList.FocusAsync();
                await sliderList.HoverAsync();
            }
            catch
            {
                // ignored
            }
        }

        return await origin.Read();
    }
}
