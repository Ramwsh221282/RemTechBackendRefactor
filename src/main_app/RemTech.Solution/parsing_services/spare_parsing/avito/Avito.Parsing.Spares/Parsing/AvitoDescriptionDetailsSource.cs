using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Parsing.Spares.Parsing;

public sealed class AvitoDescriptionDetailsSource(IPage page) : IAvitoDescriptionDetailsSource
{
    public async Task Add(AvitoSpare spare)
    {
        try
        {
            await ClickButtonIfExists();
            IElementHandle container = await new PageElementSource(page).Read(
                string.Intern("div[itemprop='description']")
            );
            IElementHandle[] paragraphs = await new ParentManyElementsSource(container).Read(
                string.Intern("p")
            );
            foreach (IElementHandle paragraph in paragraphs)
            {
                string text = await new TextFromWebElement(paragraph).Read();
                spare.AddDescriptionDetail(text.Trim());
            }
        }
        catch
        {
            // ignored
        }
    }

    private async Task ClickButtonIfExists()
    {
        try
        {
            IElementHandle container = await new PageElementSource(page).Read(
                string.Intern("div[itemprop='description']")
            );
            IElementHandle button = await new ValidSingleElementSource(
                new ParentElementSource(container)
            ).Read("a[role='button']");
            await button.ClickAsync();
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
        catch
        {
            // ignored
        }
    }
}
