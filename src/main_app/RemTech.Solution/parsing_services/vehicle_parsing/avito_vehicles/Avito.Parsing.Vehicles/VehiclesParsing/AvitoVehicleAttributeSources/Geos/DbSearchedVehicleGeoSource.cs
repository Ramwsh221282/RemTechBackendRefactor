using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.DbSearch.VehicleGeos;
using RemTech.Postgres.Adapter.Library;
using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class DbSearchedVehicleGeoSource : IParsedVehicleGeoSource
{
    private readonly PgConnectionSource _pgConnection;
    private readonly ILogger _logger;
    private readonly IParsedVehicleGeoSource _origin;

    public DbSearchedVehicleGeoSource(PgConnectionSource pgConnection, ILogger logger, IParsedVehicleGeoSource origin)
    {
        _pgConnection = pgConnection;
        _logger = logger;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleGeo> Read()
    {
        ParsedVehicleGeo geo = await _origin.Read();
        ParsedVehicleGeo fromDb = await new LoggingVehicleGeoDbSearch(_logger, 
                new VarietGeoDbSearch()
                    .With(new LazyVehicleGeoDbSearch(new TsQueryRegionDbSearch(_pgConnection)))
                    .With(new LazyVehicleGeoDbSearch(new PgTgrmRegionGeoDbSearch(_pgConnection))))
            .Search(geo.Region());
        return fromDb ? fromDb : geo;
    }
}