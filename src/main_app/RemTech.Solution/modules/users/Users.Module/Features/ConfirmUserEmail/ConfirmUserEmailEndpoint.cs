using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres;
using Shared.Infrastructure.Module.Redis;
using Users.Module.Features.ChangingEmail;

namespace Users.Module.Features.ConfirmUserEmail;

public static class ConfirmUserEmailEndpoint
{
    internal sealed record ConfirmUserEmailCommand(Guid ConfirmationId) : ICommand;

    public static void Map(RouteGroupBuilder builder) => builder.MapGet("confirm-email", Handle);

    private static async Task<IResult> Handle(
        [FromServices] PostgresDatabase dataSource,
        [FromServices] RedisCache redis,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string id,
        CancellationToken ct
    )
    {
        if (!Guid.TryParse(id, out Guid confirmationId))
            return Results.BadRequest(new { message = "Некорректный ключ подтверждения." });

        ConfirmationEmailsCache cache = new(redis);
        ConfirmUserEmailCommand command = new(confirmationId);
        ConfirmUserEmailHandler handler = new(dataSource, logger, cache);

        try
        {
            await handler.Handle(command, ct);
            return Results.Ok();
        }
        catch (UserFromConfirmationNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (ConfirmationEmailExpiredException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(ConfirmUserEmailEndpoint), ex.Message);
            return Results.InternalServerError(new { message = ex.Message });
        }
    }
}
