using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class VehicleKindFromBreadcrumbsSource : IParsedVehicleKindSource
{
    private readonly IPage _page;

    public VehicleKindFromBreadcrumbsSource(IPage page) => _page = page;

    public async Task<ParsedVehicleKind> Read()
    {
        IElementHandle[] breadCrumbs = await new AvitoVehiclePageBreadcrumbs(_page).Read();
        IElementHandle kind = breadCrumbs[^3];
        return new ParsedVehicleKind(await new TextFromWebElement(kind).Read());
    }
}
