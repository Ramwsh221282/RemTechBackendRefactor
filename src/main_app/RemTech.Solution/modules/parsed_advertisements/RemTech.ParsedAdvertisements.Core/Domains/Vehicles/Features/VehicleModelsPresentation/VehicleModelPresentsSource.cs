using System.Data.Common;
using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehicleModelsPresentation;

public sealed class VehicleModelPresentsSource(Guid brandId, Guid kindId, PgConnectionSource connectionSource)
{
    private readonly string _sql = string.Intern("""
                                                 SELECT DISTINCT m.text, m.id
                                                 FROM parsed_advertisements_module.vehicle_models m
                                                 INNER JOIN parsed_advertisements_module.parsed_vehicles v ON m.id = v.model_id
                                                 WHERE 
                                                     v.kind_id = @kind_id
                                                 AND
                                                     v.brand_id = @brand_id
                                                 """);

    public async Task<IEnumerable<VehicleModelPresent>> ReadAsync(CancellationToken ct = default)
    {
        if (kindId == Guid.Empty || brandId == Guid.Empty)
            return [];
        
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(new ParametrizingPgCommand(new PgCommand(connection, _sql))
                .With("@kind_id", kindId)
                .With("@brand_id", brandId))).AsyncReader(ct);
        return await new VehicleModelPresentsReader(reader).ReadAsync(ct);
    }
}