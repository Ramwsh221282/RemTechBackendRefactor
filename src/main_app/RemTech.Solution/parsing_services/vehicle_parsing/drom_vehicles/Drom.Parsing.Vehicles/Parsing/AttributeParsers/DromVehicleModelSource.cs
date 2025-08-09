using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.AttributeParsers;

public sealed class DromVehicleModelSource(IPage page)
{
    private readonly string _container = string.Intern("div[data-ftid='header_breadcrumb']");
    private readonly string _breadCrumbItem = string.Intern(
        "div[data-ftid='header_breadcrumb-item']"
    );

    public async Task Print(DromCatalogueCar car)
    {
        IElementHandle container = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_container);
        IElementHandle[] breadCrumbs = await new ParentManyElementsSource(container).Read(
            _breadCrumbItem
        );
        IElementHandle model = breadCrumbs[^1];
        string modelText = await new TextFromWebElement(model).Read();
        car.WithModel(modelText);
    }
}
