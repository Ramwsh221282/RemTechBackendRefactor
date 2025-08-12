using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Parsing.Spares.Parsing;

public sealed class AvitoCharacteristicsDetailsSource(IPage page) : IAvitoDescriptionDetailsSource
{
    private const string DetailsContainer = "div[data-marker='item-view/item-params']";
    private const string Ul = "ul";
    private const string Li = "li";
    private const StringSplitOptions SplitOptions = StringSplitOptions.TrimEntries;
    private const char SplitChar = ':';
    private const string Oem = "Номер запчасти";

    public async Task Add(AvitoSpare spare)
    {
        try
        {
            IElementHandle detailsContainer = await new ValidSingleElementSource(
                new PageElementSource(page)
            ).Read(DetailsContainer);
            IElementHandle ul = await new ParentElementSource(detailsContainer).Read(
                string.Intern(Ul)
            );
            IElementHandle[] rows = await new ParentManyElementsSource(ul).Read(string.Intern(Li));
            foreach (IElementHandle row in rows)
            {
                string text = await new TextFromWebElement(row).Read();
                string[] parts = text.Split(SplitChar, SplitOptions);
                string name = parts[0];
                string value = parts[1];
                if (name == Oem)
                {
                    spare.CorrectOem(value);
                    continue;
                }
                spare.AddDescriptionDetail($"{name} {value}".Trim());
            }
        }
        catch
        {
            // ignored
        }
    }
}
