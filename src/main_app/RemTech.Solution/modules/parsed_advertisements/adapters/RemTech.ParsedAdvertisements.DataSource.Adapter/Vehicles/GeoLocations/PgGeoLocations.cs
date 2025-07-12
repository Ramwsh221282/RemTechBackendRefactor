using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;

public sealed class PgGeoLocations : IAsyncGeoLocations
{
    private readonly NpgsqlDataSource _source;

    public PgGeoLocations(NpgsqlDataSource source)
    {
        _source = source;
    }

    public async Task<Status<IGeoLocation>> Add(
        IGeoLocation location,
        CancellationToken ct = default
    )
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.geos(id, text)
            VALUES(@id, @text)
            ON CONFLICT(text)
            DO NOTHING;
            """
        );
        int affected = await new AsyncExecutedCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                )
                    .With<Guid>("@id", location.Identify().ReadId())
                    .With<string>("@text", location.Identify().ReadText())
            )
        ).AsyncExecuted(ct);
        return affected == 0
            ? Error.Conflict(
                $"Геолокация с названием: {(string)location.Identify().ReadText()} уже присутствует"
            )
            : location.Success();
    }

    public async Task<MaybeBag<IGeoLocation>> Find(
        GeoLocationIdentity identity,
        CancellationToken ct = default
    )
    {
        WhereFilterSqlString filter = new WhereFilterSqlString()
            .WithIf<Guid>("id = @id", identity.ReadId(), g => g != Guid.Empty)
            .WithIf<string>(
                "text = @text",
                identity.ReadText(),
                t => !string.IsNullOrWhiteSpace(t)
            );
        if (filter.Amount() == 0)
            return new MaybeBag<IGeoLocation>();
        string sql = string.Intern(
            $"""
            SELECT id, text FROM parsed_advertisements_module.geos
            WHERE {filter.AsSqlString()}
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                )
                    .WithIf<Guid>("@id", identity.ReadId(), g => g != Guid.Empty)
                    .WithIf<string>(
                        "@text",
                        identity.ReadText(),
                        t => !string.IsNullOrWhiteSpace(t)
                    )
            )
        ).AsyncReader(ct);
        return await new MaybeSingleGeoLocationSqlRow(reader).Read(ct);
    }

    public void Dispose() => _source.Dispose();

    public async ValueTask DisposeAsync() => await _source.DisposeAsync();
}
