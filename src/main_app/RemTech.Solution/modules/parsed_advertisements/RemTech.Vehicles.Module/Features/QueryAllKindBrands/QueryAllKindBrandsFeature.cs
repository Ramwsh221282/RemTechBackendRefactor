using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryAllKindBrands;

public static class QueryAllKindBrandsFeature
{
    private const string Sql = """
        SELECT DISTINCT b.text as name, b.id as id FROM parsed_advertisements_module.parsed_vehicles v
        INNER JOIN parsed_advertisements_module.vehicle_brands b ON b.id = v.brand_id
        WHERE v.kind_id = @kindId
        ORDER BY b.text ASC;
        """;

    public sealed record QueryAllKindBrandResult(Guid Id, string Name);

    public static void Map(IEndpointRouteBuilder builder) =>
        builder.MapGet("api/brands", Handle).RequireCors("FRONTEND");

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource connectionSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] Guid kindId,
        CancellationToken ct
    )
    {
        try
        {
            if (kindId == Guid.Empty)
                return Results.NoContent();
            await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(
                ct
            );
            IEnumerable<QueryAllKindBrandResult> results = await connection.Query(kindId, ct);
            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance} {Error}.", nameof(QueryAllKindBrandsFeature), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }

    private static async Task<IEnumerable<QueryAllKindBrandResult>> Query(
        this NpgsqlConnection connection,
        Guid kindId,
        CancellationToken ct = default
    )
    {
        AsyncDbReaderCommand command = connection.FormCommand(kindId);
        await using DbDataReader reader = await command.AsyncReader(ct);
        return await reader.ReadData(ct: ct);
    }

    private static AsyncDbReaderCommand FormCommand(
        this NpgsqlConnection connection,
        Guid kindId
    ) =>
        new(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, Sql)).With("@kindId", kindId)
            )
        );

    private static async Task<IEnumerable<QueryAllKindBrandResult>> ReadData(
        this DbDataReader reader,
        CancellationToken ct = default
    )
    {
        List<QueryAllKindBrandResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            Guid id = reader.GetGuid(reader.GetOrdinal("id"));
            results.Add(new QueryAllKindBrandResult(id, name));
        }

        return results;
    }
}
