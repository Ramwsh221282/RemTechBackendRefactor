using System.Data.Common;
using Npgsql;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds;
using RemTech.ParsedAdvertisements.Core.Domains.Vehicles.Kinds.ValueObjects;
using RemTech.Postgres.Adapter.Library.PgCommands;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.DataSource.Adapter.Vehicles.Kinds;

public sealed class PgVehicleKinds : IAsyncVehicleKinds
{
    private readonly NpgsqlDataSource _source;

    public PgVehicleKinds(NpgsqlDataSource source)
    {
        _source = source;
    }

    public async Task<Status<IVehicleKind>> Add(IVehicleKind kind, CancellationToken ct = default)
    {
        string sql = string.Intern(
            """
            INSERT INTO parsed_advertisements_module.vehicle_kinds
            VALUES (@id, @text)
            ON CONFLICT (text) DO NOTHING
            """
        );
        return
            await new AsyncExecutedCommand(
                new AsyncPreparedCommand(
                    new ParametrizingPgCommand(
                        new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                    )
                        .With<Guid>("@id", kind.Identify().ReadId())
                        .With<string>("@text", kind.Identify().ReadText())
                )
            ).AsyncExecuted(ct) == 0
            ? Error.Conflict(
                $"Не удается добавить тип техники. Тип техники: {(string)kind.Identify().ReadText()} уже присутствует."
            )
            : kind.Success();
    }

    public async Task<MaybeBag<IVehicleKind>> Find(
        VehicleKindIdentity identity,
        CancellationToken ct = default
    )
    {
        List<string> searchTerms = [];
        if (identity.ReadText())
            searchTerms.Add("text = @text");
        if (identity.ReadId())
            searchTerms.Add("id = @id");
        if (searchTerms.Count == 0)
            return new MaybeBag<IVehicleKind>();
        string sql = string.Intern(
            $"""
            SELECT id, text FROM parsed_advertisements_module.vehicle_kinds
            WHERE {string.Join(" AND ", searchTerms)}
            """
        );
        await using DbDataReader reader = await new AsyncDbReaderCommand(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(
                    new PgCommand(await _source.OpenConnectionAsync(ct), sql)
                )
                    .WithIf(
                        "@text",
                        (string)identity.ReadText(),
                        t => !string.IsNullOrWhiteSpace(t)
                    )
                    .WithIf("@id", (Guid)identity.ReadId(), g => g != Guid.Empty)
            )
        ).AsyncReader(ct);
        return !await reader.ReadAsync(ct)
            ? new MaybeBag<IVehicleKind>()
            : new VehicleKindSqlRow(reader).Read().Success();
    }

    public void Dispose() => _source.Dispose();

    public ValueTask DisposeAsync() => _source.DisposeAsync();
}
