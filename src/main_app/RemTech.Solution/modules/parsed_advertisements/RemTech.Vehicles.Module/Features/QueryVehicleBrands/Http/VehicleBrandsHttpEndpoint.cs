using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Postgres.Adapter.Library;

namespace RemTech.Vehicles.Module.Features.QueryVehicleBrands.Http;

public static class VehicleBrandsHttpEndpoint
{
    public static void BrandsEndpoint(this RouteGroupBuilder group) =>
        group.MapGet("kinds/{kindId:Guid}/brands", Handle);

    private static async Task<IResult> Handle(
        [FromServices] PgConnectionSource connectionSource,
        [FromRoute] Guid kindId,
        CancellationToken ct
    )
    {
        await using NpgsqlConnection connection = await connectionSource.Connect(ct);
        return Results.Ok(
            await connection.Provide(
                kindId,
                VehicleBrandPresentationSource.CreateCommand,
                VehicleBrandPresentationSource.CreateReader,
                VehicleBrandPresentationSource.ProcessWithReader,
                ct: ct
            )
        );
    }
}
