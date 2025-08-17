using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.ContainedItems.Module.Features.DropVehicles;
using Users.Module.Public;

namespace RemTech.ContainedItems.Module.Features.DropSpares;

public static class DropSparesEndpoint
{
    public static void Map(RouteGroupBuilder builder) =>
        builder.MapPost("delete-all-spares", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] PrivelegedAccessVerify verify,
        [FromServices] Serilog.ILogger logger,
        [FromBody] DropSparesCommand command,
        CancellationToken ct
    )
    {
        try
        {
            DropSparesHandler handler = new(verify, dataSource);
            int affected = await handler.Handle(command, ct);
            logger.Information(
                "User: {Email} {Name} deleted spares: {Amount}.",
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
