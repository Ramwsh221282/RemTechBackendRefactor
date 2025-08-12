using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Parsing.Avito.Common.PaginationBar;

public sealed class AvitoPaginationBarSource(IPage page) : IAvitoPaginationBarSource
{
    private const string ButtonsSelector = ".styles-module-text-Z0vDE";
    private const string ContainerSelector = "nav[aria-label='Пагинация'";

    public async Task<AvitoPaginationBarElement> Read()
    {
        IElementHandle? container = await new PageElementSource(page).Read(ContainerSelector);
        if (container == null)
            return new AvitoPaginationBarElement([1]);

        IElementHandle[] buttons = await container.QuerySelectorAllAsync(ButtonsSelector);
        AvitoPaginationBarElement bar = new([]);
        foreach (IElementHandle button in buttons)
            if (int.TryParse(await new TextFromWebElement(button).Read(), out int number))
                bar = new AvitoPaginationBarElement(bar, number);

        return bar;
    }
}
