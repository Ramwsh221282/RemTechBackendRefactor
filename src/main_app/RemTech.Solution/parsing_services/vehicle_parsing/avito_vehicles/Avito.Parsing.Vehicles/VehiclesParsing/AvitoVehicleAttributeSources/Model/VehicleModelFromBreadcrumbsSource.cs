using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class VehicleModelFromBreadcrumbsSource : IParsedVehicleModelSource
{
    private readonly IPage _page;

    public VehicleModelFromBreadcrumbsSource(IPage page) => _page = page;

    public async Task<ParsedVehicleModel> Read()
    {
        IElementHandle[] breadCrumbs = await new AvitoVehiclePageBreadcrumbs(_page).Read();
        IElementHandle kind = breadCrumbs[^1];
        return new ParsedVehicleModel(await new TextFromWebElement(kind).Read());
    }
}
