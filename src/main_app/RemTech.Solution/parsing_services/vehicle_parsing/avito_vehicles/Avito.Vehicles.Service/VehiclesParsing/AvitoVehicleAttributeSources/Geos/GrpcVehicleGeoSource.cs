using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.Grpc.Recognition;
using Parsing.Vehicles.Grpc.Recognition.Region;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class GrpcVehicleGeoSource(IPage page, ICommunicationChannel channel)
    : IParsedVehicleGeoSource
{
    private readonly string _geoContainerSelector = string.Intern("div[itemprop='address']");
    private readonly string _geoValueSelector = string.Intern(".xLPJ6");

    public async Task<ParsedVehicleGeo> Read()
    {
        IElementHandle geoContainer = await new PageElementSource(page).Read(_geoContainerSelector);
        IElementHandle geoValue = await new ParentElementSource(geoContainer).Read(
            _geoValueSelector
        );
        string text = await new TextFromWebElement(geoValue).Read();
        Characteristic ctx = await new RegionRecognition(channel).Recognize(text);
        return !ctx
            ? throw new ArgumentException("Unable to recognize vehicle geo using grpc.")
            : new ParsedVehicleGeo(ctx.ReadValue());
    }
}
