using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation.QueryModifiers;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Features.VehiclePresentation;

public sealed class VehiclePresentsSource(
    PgConnectionSource connectionSource,
    params IVehiclePresentQueryMod[] mods)
{
    private readonly string _sql = string.Intern("""
                                   WITH aggregated_ctxes AS (
                                       SELECT vehicle_id, jsonb_agg(jsonb_build_object(
                                                        'ctx_id', ctx.ctx_id,
                                                        'ctx_name', ctx_name,
                                                        'ctx_value', ctx.ctx_value,
                                                        'ctx_measure', ctx.ctx_measure
                                                        )) as characteristics
                                       FROM parsed_advertisements_module.parsed_vehicle_characteristics ctx
                                       GROUP BY vehicle_id
                                       )
                                   SELECT
                                       v.id as vehicle_id,
                                       v.price as vehicle_price,
                                       v.photos as vehicle_photos,
                                       v.is_nds as vehicle_nds,
                                       k.id as kind_id,
                                       k.text as kind_name,
                                       b.id as brand_id,
                                       b.text as brand_name,
                                       m.id as model_id,
                                       m.text as model_name,
                                       g.id as geo_id,
                                       g.text as geo_text,
                                       c.characteristics as vehicle_characteristics
                                   FROM parsed_advertisements_module.parsed_vehicles v
                                            INNER JOIN aggregated_ctxes c ON v.id = c.vehicle_id
                                            INNER JOIN parsed_advertisements_module.vehicle_kinds k ON k.id = v.kind_id
                                            INNER JOIN parsed_advertisements_module.vehicle_brands b ON b.id = v.brand_id
                                            INNER JOIN parsed_advertisements_module.vehicle_models m ON m.id = v.model_id
                                            INNER JOIN parsed_advertisements_module.geos g on v.geo_id = g.id
                                   WHERE {0}
                                   """);
    
    public async Task<IEnumerable<VehiclePresent>> Read(CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        ParametrizingPgCommand command = new ParametrizingPgCommand(new PgCommand(connection, _sql));
        VehiclePresentQueryStorage storage = new VehiclePresentQueryStorage(command);
        foreach (var modifier in mods)
            storage = modifier.Modified(storage);

        string sql = storage.Sql();
        
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(storage.Modified()))
            .AsyncReader(ct);
        return await new VehiclePresentsReader(reader).Read(ct);
    }
}