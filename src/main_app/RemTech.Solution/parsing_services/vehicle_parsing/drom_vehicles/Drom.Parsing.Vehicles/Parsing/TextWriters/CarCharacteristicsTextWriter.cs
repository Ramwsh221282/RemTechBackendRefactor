using System.Text;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.TextWriters;

public sealed class CarCharacteristicsTextWriter(ITextWrite write, IPage page)
{
    private readonly string _containerSelector = string.Intern(".css-1bwl6o2.epjhnwz0");
    private readonly string _innerContainerSelector = string.Intern(".css-0.epjhnwz1");

    public async Task Write()
    {
        IElementHandle container = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_containerSelector);
        IElementHandle innerContainer = await new ValidSingleElementSource(
            new ParentElementSource(container)
        ).Read(_innerContainerSelector);
        IElementHandle table = await new ValidSingleElementSource(
            new ParentElementSource(innerContainer)
        ).Read(string.Intern("table"));
        IElementHandle[] tableRows = await new ParentManyElementsSource(table).Read(
            string.Intern("tr")
        );
        Dictionary<string, string> characteristics = [];
        foreach (IElementHandle row in tableRows)
        {
            try
            {
                IElementHandle ctxName = await new ValidSingleElementSource(
                    new ParentElementSource(row)
                ).Read("th");
                IElementHandle ctxValue = await new ValidSingleElementSource(
                    new ParentElementSource(row)
                ).Read("td");
                string name = await new TextFromWebElement(ctxName).Read();
                string value = await new TextFromWebElement(ctxValue).Read();
                characteristics.Add(name, value);
            }
            catch
            {
                // ignored
            }
        }

        StringBuilder sb = new StringBuilder();
        foreach (var ctx in characteristics)
        {
            sb.Append(ctx.Key).Append(' ').Append(ctx.Value).Append(' ');
        }
        await write.WriteAsync(sb.ToString().Trim());
    }
}
