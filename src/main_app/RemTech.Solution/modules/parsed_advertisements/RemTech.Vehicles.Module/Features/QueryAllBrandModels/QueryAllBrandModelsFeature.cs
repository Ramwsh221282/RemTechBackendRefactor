using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryAllBrandModels;

public static class QueryAllBrandModelsFeature
{
    private const string Sql = """
        SELECT DISTINCT m.text as name, m.id as id FROM parsed_advertisements_module.parsed_vehicles v
        INNER JOIN parsed_advertisements_module.vehicle_models m ON m.id = v.model_id
        WHERE v.brand_id = @brandId AND v.kind_id = @kindId
        ORDER BY m.text ASC;
        """;

    public sealed record QueryAllBrandModelsResult(Guid Id, string Name);

    public static void Map(IEndpointRouteBuilder builder) =>
        builder.MapGet("api/models", Handle).RequireCors("FRONTEND");

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource source,
        [FromQuery] Guid brandId,
        [FromQuery] Guid kindId,
        CancellationToken ct
    )
    {
        if (brandId == Guid.Empty || kindId == Guid.Empty)
            return Results.NoContent();
        await using NpgsqlConnection connection = await source.OpenConnectionAsync(ct);
        IEnumerable<QueryAllBrandModelsResult> result = await connection.Query(brandId, kindId, ct);
        return Results.Ok(result);
    }

    private static async Task<IEnumerable<QueryAllBrandModelsResult>> Query(
        this NpgsqlConnection connection,
        Guid brandId,
        Guid kindId,
        CancellationToken ct = default
    )
    {
        AsyncDbReaderCommand command = connection.FormCommand(brandId, kindId);
        await using DbDataReader reader = await command.AsyncReader(ct);
        return await reader.ReadData(ct: ct);
    }

    private static AsyncDbReaderCommand FormCommand(
        this NpgsqlConnection connection,
        Guid brandId,
        Guid kindId
    ) =>
        new(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, Sql))
                    .With("@brandId", brandId)
                    .With("@kindId", kindId)
            )
        );

    private static async Task<IEnumerable<QueryAllBrandModelsResult>> ReadData(
        this DbDataReader reader,
        CancellationToken ct = default
    )
    {
        List<QueryAllBrandModelsResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            Guid id = reader.GetGuid(reader.GetOrdinal("id"));
            results.Add(new QueryAllBrandModelsResult(id, name));
        }

        return results;
    }
}
