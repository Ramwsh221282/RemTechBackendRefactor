using System.Data.Common;
using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleKindsPresentation;

public sealed class VehicleKindPresentsSource
{
    private readonly PgConnectionSource _connectionSource;

    public VehicleKindPresentsSource(PgConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }

    public async Task<IEnumerable<VehicleKindPresent>> ReadAsync(CancellationToken ct = default)
    {
        string sql = string.Intern("""
                                   SELECT DISTINCT k.text, k.id 
                                   FROM parsed_advertisements_module.parsed_vehicles v
                                   INNER JOIN parsed_advertisements_module.vehicle_kinds k ON v.kind_id = k.id
                                   """);
        await using NpgsqlConnection connection = await _connectionSource.Connect(ct);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new NoParametrizingPgCommand(new PgCommand(connection, sql))))
            .AsyncReader(ct);
        return await new VehicleKindPresentReader(reader).Read(ct);
    }
}