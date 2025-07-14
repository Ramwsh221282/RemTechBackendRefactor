using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds.Decorators;

public sealed class PgTgrmPgVehicleKinds(NpgsqlDataSource source, IAsyncVehicleKinds origin)
    : IAsyncVehicleKinds
{
    public async Task<Status<IVehicleKind>> Add(IVehicleKind kind, CancellationToken ct = default)
    {
        MaybeBag<IVehicleKind> existing = await Find(kind.Identify(), ct);
        return existing.Any() ? existing.Take().Success() : await origin.Add(kind, ct);
    }

    public async Task<MaybeBag<IVehicleKind>> Find(
        VehicleKindIdentity identity,
        CancellationToken ct = default
    )
    {
        string input = identity.ReadText();
        if (string.IsNullOrEmpty(input))
            return new MaybeBag<IVehicleKind>();
        string sql = string.Intern(
            """
            SELECT id, text, word_similarity(@input, text) as sml FROM parsed_advertisements_module.vehicle_kinds
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
        return await new MaybeSingleVehicleKindSqlRow(reader).Read(ct);
    }

    public void Dispose() => origin.Dispose();

    public ValueTask DisposeAsync() => origin.DisposeAsync();
}
