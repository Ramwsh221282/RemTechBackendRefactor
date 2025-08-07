using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class VehicleBrandFromBreadcrumbsSource : IParsedVehicleBrandSource
{
    private readonly IPage _page;

    public VehicleBrandFromBreadcrumbsSource(IPage page)
    {
        _page = page;
    }

    public async Task<ParsedVehicleBrand> Read()
    {
        IElementHandle[] breadCrumbs = await new AvitoVehiclePageBreadcrumbs(_page).Read();
        IElementHandle kind = breadCrumbs[^2];
        return new ParsedVehicleBrand(await new TextFromWebElement(kind).Read());
    }
}
