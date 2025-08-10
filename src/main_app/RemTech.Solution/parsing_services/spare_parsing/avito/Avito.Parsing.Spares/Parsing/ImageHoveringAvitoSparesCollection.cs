using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Avito.Parsing.Spares.Parsing;

public sealed class ImageHoveringAvitoSparesCollection(IPage page, IAvitoSparesCollection origin)
    : IAvitoSparesCollection
{
    private readonly string _containerSelector = string.Intern("#bx_serp-item-list");
    private readonly string _itemSelector = string.Intern("div[data-marker='item']");
    private readonly string _sliderSelector = string.Intern(".iva-item-slider-BOsti");
    private readonly string _photoSelector = string.Intern(".photo-slider-list-R0jle");

    public async Task<IEnumerable<AvitoSpare>> Read()
    {
        IElementHandle container = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_containerSelector);
        IElementHandle[] items = await new ParentManyElementsSource(container).Read(_itemSelector);
        foreach (var item in items)
        {
            try
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
            catch
            {
                // ignored
            }
        }

        return await origin.Read();
    }
}
