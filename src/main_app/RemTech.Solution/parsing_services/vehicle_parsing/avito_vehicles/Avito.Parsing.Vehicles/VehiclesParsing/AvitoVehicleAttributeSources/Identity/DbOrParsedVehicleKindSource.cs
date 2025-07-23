using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.DbSearch;
using Parsing.Vehicles.DbSearch.VehicleKinds;
using RemTech.Logging.Library;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class DbOrParsedVehicleKindSource : IParsedVehicleKindSource
{
    private readonly IParsedVehicleKindSource _source;
    private readonly ICustomLogger _logger;
    private readonly ConnectionSource _connection;

    public DbOrParsedVehicleKindSource(ConnectionSource connection, ICustomLogger logger, IParsedVehicleKindSource source)
    {
        _source = source;
        _logger = logger;
        _connection = connection;
    }
    
    public async Task<ParsedVehicleKind> Read()
    {
        ParsedVehicleKind kind = await _source.Read();
        ParsedVehicleKind fromDb = await new VarietVehicleKindDbSearch()
            .With(new LoggingVehicleKindSearch(_logger, new TsQueryVehicleKindSearch(_connection)))
            .With(new LoggingVehicleKindSearch(_logger, new PgTgrmVehicleKindSearch(_connection)))
            .Search(kind);
        return fromDb ? fromDb : kind;
    }
}