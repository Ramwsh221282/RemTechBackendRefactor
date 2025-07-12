using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Brands.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Brands;

public sealed class PgVehicleBrands : IAsyncVehicleBrands
{
    private readonly NpgsqlDataSource _source;

    public PgVehicleBrands(NpgsqlDataSource source)
    {
        _source = source;
    }

    public async Task<Status<IVehicleBrand>> Add(
        IVehicleBrand brand,
        CancellationToken ct = default
    )
    {
        Guid id = brand.Identify().ReadId();
        string text = brand.Identify().ReadText();
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.vehicle_brands(id, text)
            VALUES(@id, @text)
            ON CONFLICT (text) DO NOTHING
            """
        );
        int affected = await new AsyncExecutedCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                )
                    .With("@id", id)
                    .With("@text", text)
            )
        ).AsyncExecuted(ct);
        return affected == 0
            ? Error.Conflict($"Бренд с названием: {text} уже присутствуе.")
            : brand.Success();
    }

    public async Task<MaybeBag<IVehicleBrand>> Find(
        VehicleBrandIdentity identity,
        CancellationToken ct = default
    )
    {
        WhereFilterSqlString filter = new WhereFilterSqlString()
            .WithIf<Guid>("id = @id", identity.ReadId(), g => g != Guid.Empty)
            .WithIf<string>("text = @text", identity.ReadText(), t => !string.IsNullOrEmpty(t));
        if (filter.Amount() == 0)
            return new MaybeBag<IVehicleBrand>();
        string sql = string.Intern(
            $"""
            SELECT id, text
            FROM  parsed_advertisements_module.vehicle_brands
            WHERE {filter.AsSqlString()}
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                )
                    .WithIf<Guid>("@id", identity.ReadId(), g => g != Guid.Empty)
                    .WithIf<string>("@text", identity.ReadText(), t => !string.IsNullOrEmpty(t))
            )
        ).AsyncReader(ct);
        return await new SingleVehicleBrandSqlRow(reader).Read(ct);
    }

    public void Dispose()
    {
        _source.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _source.DisposeAsync();
    }
}
