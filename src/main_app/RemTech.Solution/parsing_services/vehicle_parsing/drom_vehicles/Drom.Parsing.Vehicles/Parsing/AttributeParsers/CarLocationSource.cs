using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.AttributeParsers;

public sealed class CarLocationSource(IPage page)
{
    private readonly string _containerSelector = string.Intern(".css-inmjwf.e162wx9x0");

    public async Task<CarLocationInfo> Read()
    {
        IElementHandle[] elements = await new PageManyElementsSource(page).Read(_containerSelector);
        foreach (IElementHandle element in elements)
        {
            string text = await new TextFromWebElement(element).Read();
            if (
                string.IsNullOrWhiteSpace(text)
                || !text.Contains("Город", StringComparison.OrdinalIgnoreCase)
            )
                continue;
            text = text.Replace("Город:", string.Empty);
            return new CarLocationInfo(text);
        }

        return new CarLocationInfo(string.Empty);
    }
}
