using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands.Decorators;

public sealed class TextSearchPgVehicleBrands : IAsyncVehicleBrands
{
    private readonly NpgsqlDataSource _source;
    private readonly IAsyncVehicleBrands _origin;

    public TextSearchPgVehicleBrands(NpgsqlDataSource source, IAsyncVehicleBrands origin)
    {
        _source = source;
        _origin = origin;
    }

    public async Task<Status<IVehicleBrand>> Add(
        IVehicleBrand brand,
        CancellationToken ct = default
    )
    {
        MaybeBag<IVehicleBrand> bag = await Find(brand.Identify(), ct);
        return bag.Any() ? bag.Take().Success() : await _origin.Add(brand, ct);
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
            SELECT
            	id,
                text, 
                word_similarity  (text, @input) AS rank
            FROM 
                parsed_advertisements_module.vehicle_brands
            WHERE word_similarity  (text, @input) > 0.6
            ORDER BY rank DESC
            LIMIT 1;
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                ).With("@input", input)
            )
        ).AsyncReader(ct);
        MaybeBag<IVehicleBrand> brand = await new SingleVehicleBrandSqlRow(reader).Read(ct);
        return brand.Any() ? brand.Take().Success() : await _origin.Find(identity, ct);
    }

    public void Dispose()
    {
        _origin.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _origin.DisposeAsync();
    }
}
