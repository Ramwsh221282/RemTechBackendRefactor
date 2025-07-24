using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.DbSearch;
using Parsing.Vehicles.DbSearch.VehicleModels;
using RemTech.Logging.Library;
using RemTech.Postgres.Adapter.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class DbOrParsedVehicleModel : IParsedVehicleModelSource
{
    private readonly IParsedVehicleModelSource _origin;
    private readonly PgConnectionSource _pgConnectionSource;
    private readonly ICustomLogger _logger;

    public DbOrParsedVehicleModel(PgConnectionSource pgConnection, ICustomLogger logger, IParsedVehicleModelSource origin)
    {
        _origin = origin;
        _pgConnectionSource = pgConnection;
        _logger = logger;
    }
    
    public async Task<ParsedVehicleModel> Read()
    {
        ParsedVehicleModel model = await _origin.Read();
        ParsedVehicleModel fromDb = await new VarietVehicleModelDbSearch()
            .With(new LoggingVehicleModelDbSearch(_logger, new TsQueryVehicleModelSearch(_pgConnectionSource)))
            .With(new LoggingVehicleModelDbSearch(_logger, new PgTgrmVehicleModelDbSearch(_pgConnectionSource)))
            .Search(model);
        return fromDb ? fromDb : model;
    }
}