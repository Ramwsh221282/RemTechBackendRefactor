using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Vehicles.Module.Database.SqlStringGeneration;
using RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicles.Specifications;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCatalogue.QueryVehicleCharacteristicsDictionary;

public sealed class VehicleCharacteristicsDictionarySqlQuery
{
    private readonly string _sql = string.Intern(
        """
        SELECT
        pvc.ctx_id as ctx_id,
        pvc.ctx_name as ctx_text,
        pvc.ctx_measure as ctx_measure,
        pvc.ctx_value as ctx_value
        FROM parsed_advertisements_module.parsed_vehicles v     
        INNER JOIN parsed_advertisements_module.parsed_vehicle_characteristics pvc ON v.id = pvc.vehicle_id
        WHERE v.kind_id = @kindId AND v.brand_id = @brandId AND v.model_id = @modelId;
        """
    );

    public AsyncDbReaderCommand CreateCommand(
        Guid kindId,
        Guid brandId,
        Guid modelId,
        NpgsqlConnection connection
    )
    {
        return new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, _sql))
                    .With("@kindId", kindId)
                    .With("@brandId", brandId)
                    .With("@modelId", modelId)
            )
        );
    }
}
