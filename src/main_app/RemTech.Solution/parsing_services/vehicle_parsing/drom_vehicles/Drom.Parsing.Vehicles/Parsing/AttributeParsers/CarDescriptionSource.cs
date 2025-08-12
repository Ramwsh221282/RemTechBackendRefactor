using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.AttributeParsers;

public sealed class CarDescriptionSource(IPage page)
{
    private const string ContainerSelector = ".css-inmjwf.e162wx9x0";
    private const string InnerContainerSelector = ".css-1kb7l9z.e162wx9x0";

    public async Task Print(DromCatalogueCar car)
    {
        try
        {
            IElementHandle container = await new ValidSingleElementSource(
                new PageElementSource(page)
            ).Read(ContainerSelector);
            IElementHandle innerContainer = await new ValidSingleElementSource(
                new ParentElementSource(container)
            ).Read(InnerContainerSelector);
            string text = await new TextFromWebElement(innerContainer).Read();
            car.WithDescription(text);
        }
        catch
        {
            // ignored
        }
    }
}
