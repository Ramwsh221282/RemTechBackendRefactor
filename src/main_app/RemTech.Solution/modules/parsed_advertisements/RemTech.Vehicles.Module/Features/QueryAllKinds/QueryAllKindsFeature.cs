using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryAllKinds;

public static class QueryAllKindsFeature
{
    public sealed record QueryAllKindsResult(Guid Id, string Name);

    public static void Map(IEndpointRouteBuilder builder) =>
        builder.MapGet("api/kinds", Handle).RequireCors("FRONTEND");

    private const string Sql = """
        SELECT DISTINCT k.text as name, k.id as id FROM parsed_advertisements_module.parsed_vehicles v
        INNER JOIN parsed_advertisements_module.vehicle_kinds k ON k.id = v.kind_id
        ORDER BY k.text ASC;
        """;

    private static async Task<IResult> Handle(
        [FromServices] PgConnectionSource connectionSource,
        CancellationToken ct
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        IEnumerable<QueryAllKindsResult> result = await Query(connection, ct);
        return Results.Ok(result);
    }

    private static async Task<IEnumerable<QueryAllKindsResult>> Query(
        this NpgsqlConnection connection,
        CancellationToken ct = default
    )
    {
        AsyncDbReaderCommand command = connection.FormCommand();
        await using DbDataReader reader = await command.GetReader(ct);
        return await reader.ReadData(ct);
    }

    private static AsyncDbReaderCommand FormCommand(this NpgsqlConnection connection) =>
        new(
            new AsyncPreparedCommand(
                new DefaultPgCommandSource(new PgCommand(connection, Sql).Command())
            )
        );

    private static async Task<DbDataReader> GetReader(
        this AsyncDbReaderCommand command,
        CancellationToken ct = default
    ) => await command.AsyncReader(ct);

    private static async Task<IEnumerable<QueryAllKindsResult>> ReadData(
        this DbDataReader reader,
        CancellationToken ct = default
    )
    {
        List<QueryAllKindsResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            Guid id = reader.GetGuid(reader.GetOrdinal("id"));
            results.Add(new QueryAllKindsResult(id, name));
        }

        return results;
    }
}
