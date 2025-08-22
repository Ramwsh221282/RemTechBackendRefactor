using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Features.ChangingEmail.Exceptions;

namespace Users.Module.Features.ConfirmUserEmail;

internal sealed record ConfirmUserEmailCommand(Guid ConfirmationId) : ICommand;

public static class ConfirmUserEmailEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("confirm-email", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConfirmationEmailsCache cache,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string id,
        CancellationToken ct
    )
    {
        try
        {
            if (!Guid.TryParse(id, out Guid confirmationId))
                return Results.BadRequest(new { message = "Некорректный ключ подтверждения." });
            ConfirmUserEmailCommand command = new(confirmationId);
            ConfirmUserEmailHandler handler = new(dataSource, logger, cache);
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
