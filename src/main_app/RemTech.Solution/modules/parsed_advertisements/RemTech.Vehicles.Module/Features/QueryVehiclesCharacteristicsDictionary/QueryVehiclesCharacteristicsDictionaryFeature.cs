﻿using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library;
using RemTech.Postgres.Adapter.Library.PgCommands;

namespace RemTech.Vehicles.Module.Features.QueryVehiclesCharacteristicsDictionary;

public static class QueryVehiclesCharacteristicsDictionaryFeature
{
    private const string Sql = """
        SELECT
        pvc.ctx_id as ctx_id,
        pvc.ctx_name as ctx_text,
        pvc.ctx_measure as ctx_measure,
        pvc.ctx_value as ctx_value
        FROM parsed_advertisements_module.parsed_vehicles v     
        INNER JOIN parsed_advertisements_module.parsed_vehicle_characteristics pvc ON v.id = pvc.vehicle_id
        WHERE v.kind_id = @kindId AND v.brand_id = @brandId AND v.model_id = @modelId;
        """;

    public sealed record Characteristic(
        Guid Id,
        string Name,
        string Measure,
        HashSet<CharacteristicValue> Values
    );

    public sealed record CharacteristicValue(Guid CharacteristicId, string Value);

    public static void Map(IEndpointRouteBuilder builder) =>
        builder.MapGet("api/vehicle-characteristics", Handle).RequireCors("FRONTEND");

    private static async Task<IResult> Handle(
        [FromServices] PgConnectionSource connectionSource,
        [FromQuery] Guid kindId,
        [FromQuery] Guid brandId,
        [FromQuery] Guid modelId,
        CancellationToken ct
    )
    {
        if (kindId == Guid.Empty || brandId == Guid.Empty || modelId == Guid.Empty)
            return Results.NoContent();
        IEnumerable<Characteristic> characteristics = await ExecuteQuery(
            connectionSource,
            kindId,
            brandId,
            modelId,
            ct
        );
        return Results.Ok(characteristics);
    }

    private static async Task<IEnumerable<Characteristic>> ExecuteQuery(
        PgConnectionSource connectionSource,
        Guid kindId,
        Guid brandId,
        Guid modelId,
        CancellationToken ct
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        AsyncDbReaderCommand command = CreateCommand(connection, kindId, brandId, modelId);
        return await ExecuteCommand(command, ct);
    }

    private static AsyncDbReaderCommand CreateCommand(
        NpgsqlConnection connection,
        Guid kindId,
        Guid brandId,
        Guid modelId
    ) =>
        new(
            new AsyncPreparedCommand(
                new ParametrizingPgCommand(new PgCommand(connection, Sql))
                    .With("@kindId", kindId)
                    .With("@brandId", brandId)
                    .With("@modelId", modelId)
            )
        );

    private static async Task<IEnumerable<Characteristic>> ExecuteCommand(
        AsyncDbReaderCommand command,
        CancellationToken ct
    )
    {
        await using DbDataReader reader = await command.AsyncReader(ct);
        Dictionary<Guid, Characteristic> dictionary = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(reader.GetOrdinal("ctx_id"));
            string value = reader.GetString(reader.GetOrdinal("ctx_value"));
            if (!dictionary.TryGetValue(id, out Characteristic? ctx))
            {
                ctx = new Characteristic(
                    id,
                    reader.GetString(reader.GetOrdinal("ctx_text")),
                    reader.GetString(reader.GetOrdinal("ctx_measure")),
                    [new CharacteristicValue(id, value)]
                );
                dictionary.Add(id, ctx);
                continue;
            }
            ctx.Values.Add(new CharacteristicValue(id, value));
        }
        return dictionary.Values;
    }
}
