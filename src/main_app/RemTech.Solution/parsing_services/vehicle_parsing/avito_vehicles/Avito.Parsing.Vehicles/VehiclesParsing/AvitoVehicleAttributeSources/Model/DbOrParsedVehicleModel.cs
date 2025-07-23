using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleModels;
using Parsing.Vehicles.DbSearch;
using Parsing.Vehicles.DbSearch.VehicleModels;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Model;

public sealed class DbOrParsedVehicleModel : IParsedVehicleModelSource
{
    private readonly IParsedVehicleModelSource _origin;
    private readonly ConnectionSource _connectionSource;
    private readonly ICustomLogger _logger;

    public DbOrParsedVehicleModel(ConnectionSource connection, ICustomLogger logger, IParsedVehicleModelSource origin)
    {
        _origin = origin;
        _connectionSource = connection;
        _logger = logger;
    }
    
    public async Task<ParsedVehicleModel> Read()
    {
        ParsedVehicleModel model = await _origin.Read();
        ParsedVehicleModel fromDb = await new VarietVehicleModelDbSearch()
            .With(new LoggingVehicleModelDbSearch(_logger, new TsQueryVehicleModelSearch(_connectionSource)))
            .With(new LoggingVehicleModelDbSearch(_logger, new PgTgrmVehicleModelDbSearch(_connectionSource)))
            .Search(model);
        return fromDb ? fromDb : model;
    }
}