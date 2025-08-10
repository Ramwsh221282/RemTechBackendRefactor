using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.Parsing;

public sealed class AvitoCharacteristicsDetailsSource(IPage page) : IAvitoDescriptionDetailsSource
{
    public async Task Add(AvitoSpare spare)
    {
        try
        {
            IElementHandle detailsContainer = await new ValidSingleElementSource(
                new PageElementSource(page)
            ).Read(string.Intern("div[data-marker='item-view/item-params']"));
            IElementHandle ul = await new ParentElementSource(detailsContainer).Read(
                string.Intern("ul")
            );
            IElementHandle[] rows = await new ParentManyElementsSource(ul).Read(
                string.Intern("li")
            );
            foreach (IElementHandle row in rows)
            {
                string text = await new TextFromWebElement(row).Read();
                string[] parts = text.Split(':', StringSplitOptions.TrimEntries);
                string name = parts[0];
                string value = parts[1];
                if (name == "Номер запчасти")
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
