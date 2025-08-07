using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Model;

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
