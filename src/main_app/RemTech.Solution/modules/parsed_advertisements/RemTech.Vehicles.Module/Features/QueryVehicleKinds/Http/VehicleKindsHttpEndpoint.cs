using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.Vehicles.Module.Features.QueryVehicleKinds.Http;

public static class VehicleKindsHttpEndpoint
{
    public static void KindsEndpoint(this RouteGroupBuilder builder) =>
        builder.MapGet("kinds", Handle);

    private static async Task<IResult> Handle(
        [FromServices] PgConnectionSource connectionSource,
        CancellationToken ct
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return Results.Ok(
            await connection.Provide(
                VehicleKindsPresentationSource.VehicleKindsCommand,
                VehicleKindsPresentationSource.VehicleKindsReader,
                VehicleKindsPresentationSource.VehicleKindsReading,
                ct
            )
        );
    }
}
