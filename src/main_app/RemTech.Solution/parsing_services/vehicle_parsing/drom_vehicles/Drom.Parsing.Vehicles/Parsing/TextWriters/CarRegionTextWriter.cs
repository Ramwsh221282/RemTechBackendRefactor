using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.TextWriters;

public sealed class CarRegionTextWriter(ITextWrite write, IPage page)
{
    private readonly string _containerSelector = string.Intern(".css-inmjwf.e162wx9x0");

    public async Task Write()
    {
        IElementHandle[] elements = await new PageManyElementsSource(page).Read(_containerSelector);
        foreach (IElementHandle element in elements)
        {
            string text = await new TextFromWebElement(element).Read();
            if (
                !string.IsNullOrWhiteSpace(text)
                && text.Contains("Город", StringComparison.OrdinalIgnoreCase)
            )
                await write.WriteAsync(text);
        }
    }
}
