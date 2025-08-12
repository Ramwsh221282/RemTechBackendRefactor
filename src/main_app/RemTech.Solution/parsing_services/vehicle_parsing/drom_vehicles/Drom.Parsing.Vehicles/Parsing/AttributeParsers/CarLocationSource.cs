using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.AttributeParsers;

public sealed class CarLocationSource(IPage page)
{
    private const string ContainerSelector = ".css-inmjwf.e162wx9x0";
    private const string City = "Город";
    private const string Replace = "Город:";
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    public async Task<CarLocationInfo> Read()
    {
        IElementHandle[] elements = await new PageManyElementsSource(page).Read(ContainerSelector);
        foreach (IElementHandle element in elements)
        {
            string text = await new TextFromWebElement(element).Read();
            if (string.IsNullOrWhiteSpace(text) || !text.Contains(City, Comparison))
                continue;
            text = text.Replace(Replace, string.Empty);
            return new CarLocationInfo(text);
        }

        return new CarLocationInfo(string.Empty);
    }
}
