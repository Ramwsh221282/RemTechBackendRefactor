using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;

public sealed class TgrmPgGeoLocations(NpgsqlDataSource source, IAsyncGeoLocations locations)
    : IAsyncGeoLocations
{
    public async Task<Status<IGeoLocation>> Add(
        IGeoLocation location,
        CancellationToken ct = default
    )
    {
        MaybeBag<IGeoLocation> existing = await Find(location.Identify(), ct);
        return existing.Any() ? existing.Take().Success() : await locations.Add(location, ct);
    }

    public async Task<MaybeBag<IGeoLocation>> Find(
        GeoLocationIdentity identity,
        CancellationToken ct = default
    )
    {
        string input = identity.ReadText();
        if (string.IsNullOrWhiteSpace(input))
            return new MaybeBag<IGeoLocation>();
        string sql = string.Intern(
            """
            SELECT id, text, kind, word_similarity(@input, text) as sml FROM parsed_advertisements_module.geos
            WHERE word_similarity(@input, text) > 0.8
            ORDER BY sml DESC
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await source.OpenConnectionAsync(ct), sql)
                ).With("@input", input)
            )
        ).AsyncReader(ct);
        MaybeBag<IGeoLocation> location = await new MaybeSingleGeoLocationSqlRow(reader).Read(ct);
        return location.Any() ? location.Take().Success() : await locations.Find(identity, ct);
    }

    public void Dispose() => source.Dispose();

    public ValueTask DisposeAsync() => source.DisposeAsync();
}
