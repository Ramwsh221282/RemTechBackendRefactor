using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.AttributeParsers;

public sealed class CarDescriptionSource(IPage page)
{
    private readonly string _containerSelector = string.Intern(".css-inmjwf.e162wx9x0");
    private readonly string _innerContainerSelector = string.Intern(".css-1kb7l9z.e162wx9x0");

    public async Task Print(DromCatalogueCar car)
    {
        try
        {
            IElementHandle container = await new ValidSingleElementSource(
                new PageElementSource(page)
            ).Read(_containerSelector);
            IElementHandle innerContainer = await new ValidSingleElementSource(
                new ParentElementSource(container)
            ).Read(_innerContainerSelector);
            string text = await new TextFromWebElement(innerContainer).Read();
            car.WithDescription(text);
        }
        catch
        {
            // ignored
        }
    }
}
