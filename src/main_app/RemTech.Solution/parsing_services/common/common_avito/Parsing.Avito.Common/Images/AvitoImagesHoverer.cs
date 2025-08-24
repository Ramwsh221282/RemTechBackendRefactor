using Parsing.SDK.ElementSources;
using PuppeteerSharp;

namespace Parsing.Avito.Common.Images;

public sealed class AvitoImagesHoverer(IPage page)
{
    private const string ContainerSelector = "div[data-marker='catalog-serp']";
    private const string ItemsSelector = "div[data-marker='item']";
    private const string SliderSelector = ".iva-item-slider-BOsti";

    public async Task HoverImages()
    {
        await Task.Delay(TimeSpan.FromSeconds(10));
        bool hovered = false;
        for (int i = 0; i < 5; i++)
        {
            try
            {
                await Hover();
                hovered = true;
                break;
            }
            catch
            {
                Console.WriteLine("Error hovering item. Retrying...");
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }

        if (!hovered)
            throw new ApplicationException("Exception at image hovering.");
        await Task.Delay(TimeSpan.FromSeconds(10));
    }

    private async Task Hover()
    {
        IElementHandle container = await new PageElementSource(page).Read(
            string.Intern(ContainerSelector)
        );
        IElementHandle[] itemsList = await new ParentManyElementsSource(container).Read(
            string.Intern(ItemsSelector)
        );
        foreach (IElementHandle item in itemsList)
        {
            await using (item)
            {
                try
                {
                    IElementHandle sliderElement = await new ValidSingleElementSource(
                        new ParentElementSource(item)
                    ).Read(SliderSelector);
                    await sliderElement.HoverAsync();
                    await sliderElement.FocusAsync();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
