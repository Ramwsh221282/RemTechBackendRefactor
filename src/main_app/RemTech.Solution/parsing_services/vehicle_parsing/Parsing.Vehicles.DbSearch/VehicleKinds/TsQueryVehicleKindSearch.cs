using System.Data.Common;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace Parsing.Vehicles.DbSearch.VehicleKinds;

public sealed class TsQueryVehicleKindSearch : IVehicleKindDbSearch
{
    private readonly ConnectionSource _connectionSource;

    public TsQueryVehicleKindSearch(ConnectionSource connectionSource)
    {
        _connectionSource = connectionSource;
    }

    public async Task<ParsedVehicleKind> Search(string text)
    {
        string sql = string.Intern("""
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
                                           ts_rank(vehicle_kinds.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                                       FROM parsed_advertisements_module.vehicle_kinds
                                       WHERE vehicle_kinds.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                                       ORDER BY rank DESC
                                       LIMIT 1
                                       ) e ON true
                                            LEFT JOIN LATERAL (
                                       SELECT ts_rank(vehicle_kinds.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                                       FROM parsed_advertisements_module.vehicle_kinds
                                       WHERE vehicle_kinds.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                                       ORDER BY rank DESC
                                       LIMIT 1
                                       ) max_sim ON true
                                   WHERE iw.input_word != '' AND max_sim.rank > 0
                                   ORDER BY rank DESC
                                   LIMIT 1;
                                   """);
        await using DbDataReader reader = await new AsyncDbReaderCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(
                            new PgCommand(await _connectionSource.Connect(), sql))
                        .With("@input", text)))
            .AsyncReader();
        return await new SearchedVehicleKind(reader).Read();
    }
}