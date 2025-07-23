using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.DbSearch;
using Parsing.Vehicles.DbSearch.VehicleGeos;
using PuppeteerSharp;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class DbSearchedVehicleGeoSource : IParsedVehicleGeoSource
{
    private readonly ConnectionSource _connection;
    private readonly ICustomLogger _logger;
    private readonly IParsedVehicleGeoSource _origin;

    public DbSearchedVehicleGeoSource(ConnectionSource connection, ICustomLogger logger, IParsedVehicleGeoSource origin)
    {
        _connection = connection;
        _logger = logger;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleGeo> Read()
    {
        ParsedVehicleGeo geo = await _origin.Read();
        ParsedVehicleGeo fromDb = await new LoggingVehicleGeoDbSearch(_logger,
                new VarietGeoDbSearch()
                    .With(new PgTgrmRegionCityDbSearch(_connection,
                        new LazyVehicleGeoDbSearch(new PgTgrmRegionGeoDbSearch(_connection))))
                    .With(new TsQueryRegionCitySearch(_connection,
                        new LazyVehicleGeoDbSearch(new TsQueryRegionDbSearch(_connection)))))
            .Search(geo.Region());
        return fromDb ? fromDb : geo;
    }
}

public sealed class DefaultOnErroVehicleGeo : IParsedVehicleGeoSource
{
    private readonly IParsedVehicleGeoSource _origin;

    public DefaultOnErroVehicleGeo(IParsedVehicleGeoSource origin)
    {
        _origin = origin;
    }
    
    public async Task<ParsedVehicleGeo> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return new ParsedVehicleGeo(new ParsedVehicleRegion(), new ParsedVehicleCity());
        }
    }
}

public sealed class WebElementVehicleGeoSource : IParsedVehicleGeoSource
{
    private readonly IPage _page;
    private readonly string _geoContainerSelector = string.Intern("div[itemprop='address']");
    private readonly string _geoValueSelector = string.Intern(".xLPJ6");
    private readonly StringSplitOptions _splitOptions =
        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries; 

    public WebElementVehicleGeoSource(IPage page)
    {
        _page = page;
    }
    
    public async Task<ParsedVehicleGeo> Read()
    {
        IElementHandle geoContainer = await new PageElementSource(_page).Read(_geoContainerSelector);
        IElementHandle geoValue = await new ParentElementSource(geoContainer).Read(_geoValueSelector);
        string text = await new TextFromWebElement(geoValue).Read();
        string[] textParts = text.Split(',', _splitOptions);
        string regionPart = textParts[0];
        string[] regionParts = regionPart.Split(' ', _splitOptions);
        string region = regionParts[0];
        return new ParsedVehicleGeo(new ParsedVehicleRegion(region), new ParsedVehicleCity());
    }
}