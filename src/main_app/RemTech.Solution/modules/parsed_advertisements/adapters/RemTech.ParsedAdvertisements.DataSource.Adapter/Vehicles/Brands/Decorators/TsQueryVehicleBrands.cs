using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands.Decorators;

public sealed class TsQueryVehicleBrands(NpgsqlDataSource source, IAsyncVehicleBrands origin)
    : IAsyncVehicleBrands
{
    public async Task<Status<IVehicleBrand>> Add(
        IVehicleBrand brand,
        CancellationToken ct = default
    )
    {
        MaybeBag<IVehicleBrand> bag = await Find(brand.Identify(), ct);
        return bag.Any() ? bag.Take().Success() : await origin.Add(brand, ct);
    }

    public async Task<MaybeBag<IVehicleBrand>> Find(
        VehicleBrandIdentity identity,
        CancellationToken ct = default
    )
    {
        string text = identity.ReadText();
        if (string.IsNullOrEmpty(text))
            return await origin.Find(identity, ct);
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
                    ts_rank(vehicle_brands.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.vehicle_brands
                WHERE vehicle_brands.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) e ON true
                     LEFT JOIN LATERAL (
                SELECT ts_rank(vehicle_brands.document_tsvector, to_tsquery('russian', lower(iw.input_word))) AS rank
                FROM parsed_advertisements_module.vehicle_brands
                WHERE vehicle_brands.document_tsvector @@ to_tsquery('russian', lower(iw.input_word))
                ORDER BY rank DESC
                LIMIT 1
                ) max_sim ON true
            WHERE iw.input_word != '' AND max_sim.rank > 0
            ORDER BY rank DESC
            LIMIT 1;
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await source.OpenConnectionAsync(ct), sql)
                ).With("@input", text)
            )
        ).AsyncReader(ct);
        MaybeBag<IVehicleBrand> brand = await new SingleVehicleBrandSqlRow(reader).Read(ct);
        return brand.Any() ? brand.Take().Success() : await origin.Find(identity, ct);
    }

    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();
}
