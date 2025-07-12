using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.GeoLocations.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.GeoLocations;

public sealed class TextSearchPgGeoLocations(NpgsqlDataSource source, IAsyncGeoLocations locations)
    : IAsyncGeoLocations
{
    public void Dispose() => locations.Dispose();

    public ValueTask DisposeAsync() => locations.DisposeAsync();

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
        string text = identity.ReadText();
        if (string.IsNullOrEmpty(text))
            return await locations.Find(identity, ct);
        string sql = string.Intern(
            """
            SELECT 
                id,
                text,
                word_similarity(text, @input) as sml 
            FROM parsed_advertisements_module.geos
            WHERE word_similarity(text, @input) > 0.5
            ORDER BY sml DESC
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
        return await new MaybeSingleGeoLocationSqlRow(reader).Read(ct);
    }
}
