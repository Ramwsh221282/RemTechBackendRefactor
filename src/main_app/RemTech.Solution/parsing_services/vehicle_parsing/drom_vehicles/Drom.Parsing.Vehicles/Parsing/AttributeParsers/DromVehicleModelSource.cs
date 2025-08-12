using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.AttributeParsers;

public sealed class DromVehicleModelSource(IPage page)
{
    private const string Container = "div[data-ftid='header_breadcrumb']";
    private const string BreadCrumbItem = "div[data-ftid='header_breadcrumb-item']";

    public async Task Print(DromCatalogueCar car)
    {
        IElementHandle container = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(Container);
        IElementHandle[] breadCrumbs = await new ParentManyElementsSource(container).Read(
            BreadCrumbItem
        );
        IElementHandle model = breadCrumbs[^1];
        string modelText = await new TextFromWebElement(model).Read();
        car.WithModel(modelText);
    }
}
