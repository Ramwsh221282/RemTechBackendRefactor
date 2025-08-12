using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Parsing.Spares.Parsing;

public sealed class AvitoDescriptionDetailsSource(IPage page) : IAvitoDescriptionDetailsSource
{
    private const string Container = "div[itemprop='description']";
    private const string P = "p";
    private const string Button = "a[role='button']";

    public async Task Add(AvitoSpare spare)
    {
        try
        {
            await ClickButtonIfExists();
            IElementHandle container = await new PageElementSource(page).Read(Container);
            IElementHandle[] paragraphs = await new ParentManyElementsSource(container).Read(P);
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
            IElementHandle container = await new PageElementSource(page).Read(Container);
            IElementHandle button = await new ValidSingleElementSource(
                new ParentElementSource(container)
            ).Read(Button);
            await button.ClickAsync();
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
        catch
        {
            // ignored
        }
    }
}
