using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.DbSearch;
using Parsing.Vehicles.DbSearch.VehicleBrands;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class DbOrParsedVehicleBrandSource : IParsedVehicleBrandSource
{
    private readonly ConnectionSource _connectionSource;
    private readonly IParsedVehicleBrandSource _origin;
    private readonly ICustomLogger _logger;

    public DbOrParsedVehicleBrandSource(ConnectionSource connectionSource, ICustomLogger logger, IParsedVehicleBrandSource origin)
    {
        _connectionSource = connectionSource;
        _logger = logger;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleBrand> Read()
    {
        ParsedVehicleBrand current = await _origin.Read();
        ParsedVehicleBrand fromDb = await new VarietVehicleBrandDbSearch()
            .With(new LoggingVehicleBrandDbSearch(_logger, new TsQueryVehicleBrandDbSearch(_connectionSource)))
            .With(new LoggingVehicleBrandDbSearch(_logger, new PgTgrmVehicleBrandDbSearch(_connectionSource)))
            .Search(current);
        return fromDb ? fromDb : current;
    }
}