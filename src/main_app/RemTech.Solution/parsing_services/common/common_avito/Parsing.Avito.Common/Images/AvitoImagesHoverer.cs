using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingActions;
using PuppeteerSharp;

namespace Parsing.Avito.Common.Images;

public sealed class AvitoImagesHoverer(IPage page)
{
    private const string ContainerSelector = "#bx_serp-item-list";
    private const string ItemSelector = "div[data-marker='item']";
    private const string SliderSelector = ".photo-slider-list-R0jle";
    private const string PhotoSelector = "li";

    public async Task HoverImages()
    {
        // IElementHandle[] items = await GetItems();
        // await HoverItems(items);
    }

    private async Task<IElementHandle[]> GetItems()
    {
        IElementHandle container = await new PageElementSource(page).Read(ContainerSelector);
        return await new ParentManyElementsSource(container).Read(ItemSelector);
    }

    private async Task HoverItems(IElementHandle[] items)
    {
        await new PageBottomScrollingAction(page).Do();
        foreach (IElementHandle item in items)
        {
            IElementHandle slider = await new ValidSingleElementSource(
                new ParentElementSource(item)
            ).Read(SliderSelector);
            IElementHandle[] elements = await new ParentManyElementsSource(slider).Read(
                PhotoSelector
            );

            await item.EvaluateFunctionHandleAsync(
                "element => element.scrollIntoView({ behavior: 'smooth', block: 'center' })"
            );

            await item.FocusAsync();
            foreach (var image in elements)
            {
                try
                {
                    IElementHandle img = await new ValidSingleElementSource(
                        new ParentElementSource(image)
                    ).Read(string.Intern("img"));
                    await img.FocusAsync();
                }
                catch { }
            }
        }
    }
}
