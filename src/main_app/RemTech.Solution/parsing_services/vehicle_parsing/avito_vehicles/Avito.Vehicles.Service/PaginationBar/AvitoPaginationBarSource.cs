using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.PaginationBar;

public sealed class AvitoPaginationBarSource(IPage page) : IAvitoPaginationBarSource
{
    private readonly string _buttonsSelector = string.Intern(".styles-module-text-Z0vDE");
    private readonly string _containerSelector = string.Intern("nav[aria-label='Пагинация'");

    public async Task<AvitoPaginationBarElement> Read()
    {
        IElementHandle? container = await new PageElementSource(page).Read(_containerSelector);
        if (container == null)
            return new AvitoPaginationBarElement([1]);

        IElementHandle[] buttons = await container.QuerySelectorAllAsync(_buttonsSelector);
        AvitoPaginationBarElement bar = new([]);
        foreach (IElementHandle button in buttons)
            if (int.TryParse(await new TextFromWebElement(button).Read(), out int number))
                bar = new AvitoPaginationBarElement(bar, number);

        return bar;
    }
}
