using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleBrands;
using Parsing.Vehicles.DbSearch.VehicleBrands;
using RemTech.Postgres.Adapter.Library;
using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Brand;

public sealed class DbOrParsedVehicleBrandSource : IParsedVehicleBrandSource
{
    private readonly PgConnectionSource _pgConnectionSource;
    private readonly IParsedVehicleBrandSource _origin;
    private readonly ILogger _logger;

    public DbOrParsedVehicleBrandSource(PgConnectionSource pgConnectionSource, ILogger logger, IParsedVehicleBrandSource origin)
    {
        _pgConnectionSource = pgConnectionSource;
        _logger = logger;
        _origin = origin;
    }
    
    public async Task<ParsedVehicleBrand> Read()
    {
        ParsedVehicleBrand current = await _origin.Read();
        ParsedVehicleBrand fromDb = await new VarietVehicleBrandDbSearch()
            .With(new LoggingVehicleBrandDbSearch(_logger, new TsQueryVehicleBrandDbSearch(_pgConnectionSource)))
            .With(new LoggingVehicleBrandDbSearch(_logger, new PgTgrmVehicleBrandDbSearch(_pgConnectionSource)))
            .Search(current);
        return fromDb ? fromDb : current;
    }
}