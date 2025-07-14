using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands.Decorators;

public sealed class PgTgrmVehicleBrands(NpgsqlDataSource source, IAsyncVehicleBrands origin)
    : IAsyncVehicleBrands
{
    public async Task<Status<IVehicleBrand>> Add(
        IVehicleBrand brand,
        CancellationToken ct = default
    )
    {
        var existing = await Find(brand.Identify(), ct);
        return existing.Any() ? existing.Take().Success() : await origin.Add(brand, ct);
    }

    public async Task<MaybeBag<IVehicleBrand>> Find(
        VehicleBrandIdentity identity,
        CancellationToken ct = default
    )
    {
        string input = identity.ReadText();
        if (string.IsNullOrEmpty(input))
            return new MaybeBag<IVehicleBrand>();
        string sql = string.Intern(
            """
            SELECT id, text, word_similarity(@input, text) as sml
            FROM parsed_advertisements_module.vehicle_brands
            WHERE word_similarity(@input, text) > 0.8
            ORDER BY sml DESC
            LIMIT 1;
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await source.OpenConnectionAsync(ct), sql)
                ).With("@input", input)
            )
        ).AsyncReader(ct);
        MaybeBag<IVehicleBrand> brand = await new SingleVehicleBrandSqlRow(reader).Read(ct);
        return brand.Any() ? brand.Take().Success() : await origin.Find(identity, ct);
    }

    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();
}
