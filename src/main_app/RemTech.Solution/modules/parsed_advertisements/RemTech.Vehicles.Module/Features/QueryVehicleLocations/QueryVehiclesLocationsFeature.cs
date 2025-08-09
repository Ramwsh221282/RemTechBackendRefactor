using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehicleLocations;

public static class QueryVehiclesLocationsFeature
{
    private const string Sql = """
        SELECT DISTINCT
        g.id as location_id,
        g.text as location_name,
        g.kind as location_kind
        FROM parsed_advertisements_module.parsed_vehicles v
        INNER JOIN parsed_advertisements_module.geos g ON g.id = v.geo_id
        WHERE v.kind_id = @kindId AND v.brand_id = @brandId
        ORDER BY g.text ASC;
        """;

    public sealed record QueryVehiclesLocationsResult(Guid Id, string Name, string Kind);

    public static void Map(IEndpointRouteBuilder builder) =>
        builder.MapGet("api/locations", Handle).RequireCors("FRONTEND");

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource connectionSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] Guid kindId,
        [FromQuery] Guid brandId,
        CancellationToken ct
    )
    {
        try
        {
            if (kindId == Guid.Empty || brandId == Guid.Empty)
                return Results.NoContent();
            await using NpgsqlConnection connection = await connectionSource.OpenConnectionAsync(
                ct
            );
            IEnumerable<QueryVehiclesLocationsResult> result = await ExecuteQuery(
                connection,
                kindId,
                brandId,
                ct
            );
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Error}.", nameof(QueryVehiclesLocationsFeature), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }

    private static async Task<IEnumerable<QueryVehiclesLocationsResult>> ExecuteQuery(
        NpgsqlConnection connection,
        Guid kindId,
        Guid brandId,
        CancellationToken ct
    )
    {
        AsyncDbReaderCommand command = CreateCommand(connection, kindId, brandId);
        await using DbDataReader reader = await command.AsyncReader(ct);
        return await ProcessReader(reader, ct);
    }

    private static AsyncDbReaderCommand CreateCommand(
        NpgsqlConnection connection,
        Guid kindId,
        Guid brandId
    ) =>
        new(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, Sql))
                    .With("@kindId", kindId)
                    .With("@brandId", brandId)
            )
        );

    private static async Task<IEnumerable<QueryVehiclesLocationsResult>> ProcessReader(
        DbDataReader reader,
        CancellationToken ct = default
    )
    {
        List<QueryVehiclesLocationsResult> results = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(reader.GetOrdinal("location_id"));
            string name = reader.GetString(reader.GetOrdinal("location_name"));
            string kind = reader.GetString(reader.GetOrdinal("location_kind"));
            QueryVehiclesLocationsResult result = new(id, name, kind);
            results.Add(result);
        }
        return results;
    }
}
