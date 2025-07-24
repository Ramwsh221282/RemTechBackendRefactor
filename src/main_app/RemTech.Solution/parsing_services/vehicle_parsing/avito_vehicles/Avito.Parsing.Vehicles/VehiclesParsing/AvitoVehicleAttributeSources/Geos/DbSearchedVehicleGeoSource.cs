using Npgsql;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleGeo;
using Parsing.Vehicles.DbSearch;
using Parsing.Vehicles.DbSearch.VehicleGeos;
using RemTech.Logging.Library;
using RemTech.Postgres.Adapter.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Geos;

public sealed class DbSearchedVehicleGeoSource : IParsedVehicleGeoSource
{
    private readonly PgConnectionSource _pgConnection;
    private readonly ICustomLogger _logger;
    private readonly IParsedVehicleGeoSource _origin;

    public DbSearchedVehicleGeoSource(PgConnectionSource pgConnection, ICustomLogger logger, IParsedVehicleGeoSource origin)
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
                    .With(new TsQueryRegionCitySearch(_pgConnection,
                        new LazyVehicleGeoDbSearch(new PgTgrmRegionGeoDbSearch(_pgConnection))))
                    .With(new PgTgrmRegionCityDbSearch(_pgConnection,
                        new LazyVehicleGeoDbSearch(new TsQueryRegionDbSearch(_pgConnection)))))
            .Search(geo.Region());
        return fromDb ? fromDb : geo;
    }
}