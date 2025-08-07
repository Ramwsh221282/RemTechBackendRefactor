using System.Data.Common;
using Npgsql;
using RemTech.Vehicles.Module.Types.Models.ValueObjects;

namespace RemTech.Vehicles.Module.Types.Models.Storage;

internal sealed class TsQueryVehicleModelsStorage(NpgsqlDataSource dataSource)
    : IVehicleModelsStorage
{
    public async Task<VehicleModel> Store(VehicleModel vehicleModel)
    {
        string sql = string.Intern(
            """
            WITH input_words AS (
                SELECT
                    unnest(string_to_array(
                            lower(regexp_replace(@input, '[^a-zA-Z0-9А-Яа-я ]', '', 'g')),
                            ' '
                           )) AS input_word
            )
            SELECT
                COALESCE(e.text, 'Не найдено') AS text,
                COALESCE(max_sim.rank, 0) AS rank,
                e.id AS id
            FROM input_words iw
                     LEFT JOIN LATERAL (
                SELECT
                    id,
                    text,
                    ts_rank(vehicle_models.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.vehicle_models
                WHERE vehicle_models.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) e ON true
                     LEFT JOIN LATERAL (
                SELECT ts_rank(vehicle_models.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.vehicle_models
                WHERE vehicle_models.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) max_sim ON true
            WHERE iw.input_word != '' AND max_sim.rank > 0
            ORDER BY rank DESC
            LIMIT 1;   
            """
        );
        string name = vehicleModel.NameString();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<string>("@input", name));
        await using DbDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new UnableToStoreVehicleModelException(
                "Не удается получить модель техники по ts query.",
                name
            );
        Guid id = reader.GetGuid(reader.GetOrdinal("id"));
        string text = reader.GetString(reader.GetOrdinal("text"));
        VehicleModelIdentity identity = new VehicleModelIdentity(id);
        VehicleModelName modelName = new VehicleModelName(text);
        return vehicleModel.ChangeIdentity(identity).Rename(modelName);
    }
}
