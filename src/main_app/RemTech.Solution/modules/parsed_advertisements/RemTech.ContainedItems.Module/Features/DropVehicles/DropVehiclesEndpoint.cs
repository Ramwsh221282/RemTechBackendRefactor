using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Users.Module.Public;

namespace RemTech.ContainedItems.Module.Features.DropVehicles;

public static class DropVehiclesEndpoint
{
    public static void Map(RouteGroupBuilder builder) =>
        builder.MapPost("delete-all-vehicles", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] PrivelegedAccessVerify verify,
        [FromServices] Serilog.ILogger logger,
        [FromBody] DropVehiclesCommand command,
        CancellationToken ct
    )
    {
        try
        {
            DropVehiclesHandler handler = new(verify, dataSource);
            int affected = await handler.Handle(command, ct);
            logger.Information(
                "User: {Email} {Name} deleted vehicles: {Amount}.",
                command.Email,
                command.Name,
                affected
            );
            return Results.Ok(affected);
        }
        catch (DropContainedItemsDeniedException ex)
        {
            logger.Warning(
                "User: {Email} {Name} deleted vehicles. Denied",
                command.Email,
                command.Name
            );
            return Results.BadRequest(new { message = ex.Message });
        }
    }
}
