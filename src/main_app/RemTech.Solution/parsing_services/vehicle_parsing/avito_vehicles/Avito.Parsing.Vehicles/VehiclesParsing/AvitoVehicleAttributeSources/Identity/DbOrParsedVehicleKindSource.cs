using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.DbSearch.VehicleKinds;
using RemTech.Postgres.Adapter.Library;
using Serilog;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Identity;

public sealed class DbOrParsedVehicleKindSource : IParsedVehicleKindSource
{
    private readonly IParsedVehicleKindSource _source;
    private readonly ILogger _logger;
    private readonly PgConnectionSource _pgConnection;

    public DbOrParsedVehicleKindSource(PgConnectionSource pgConnection, ILogger logger, IParsedVehicleKindSource source)
    {
        _source = source;
        _logger = logger;
        _pgConnection = pgConnection;
    }
    
    public async Task<ParsedVehicleKind> Read()
    {
        ParsedVehicleKind kind = await _source.Read();
        ParsedVehicleKind fromDb = await new VarietVehicleKindDbSearch()
            .With(new LoggingVehicleKindSearch(_logger, new TsQueryVehicleKindSearch(_pgConnection)))
            .With(new LoggingVehicleKindSearch(_logger, new PgTgrmVehicleKindSearch(_pgConnection)))
            .Search(kind);
        return fromDb ? fromDb : kind;
    }
}