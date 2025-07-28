using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.DbSearch.VehicleModels;
using RemTech.Postgres.Adapter.Library;
using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class DbOrParsedVehicleModel : IParsedVehicleModelSource
{
    private readonly IParsedVehicleModelSource _origin;
    private readonly PgConnectionSource _pgConnectionSource;
    private readonly ILogger _logger;

    public DbOrParsedVehicleModel(PgConnectionSource pgConnection, ILogger logger, IParsedVehicleModelSource origin)
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