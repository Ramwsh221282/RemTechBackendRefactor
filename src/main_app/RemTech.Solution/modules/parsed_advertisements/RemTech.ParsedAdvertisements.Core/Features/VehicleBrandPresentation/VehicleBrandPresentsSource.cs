using System.Data.Common;
using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Features.VehicleBrandPresentation;

public sealed class VehicleBrandPresentsSource(Guid kindId, PgConnectionSource connectionSource)
{
    public async Task<IEnumerable<VehicleBrandPresent>> ReadAsync(CancellationToken ct = default)
    {
        if (kindId == Guid.Empty)
            return [];
        string sql = string.Intern(
            """
            SELECT DISTINCT b.text, b.id 
            FROM parsed_advertisements_module.parsed_vehicles v
            INNER JOIN parsed_advertisements_module.vehicle_brands b ON v.brand_id = b.id
            WHERE v.kind_id = @kind_id;
            """
        );
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, sql)).With("@kind_id", kindId)
            )
        ).AsyncReader();
        return await new VehicleBrandPresentsReader(reader).ReadAsync(ct);
    }
}
