using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Domain.JournalsContext.Exceptions;

namespace Scrapers.Module.Domain.JournalsContext.Features.RemoveScraperJournal;

public static class RemoveScraperJournalEndpoint
{
    public static void Map(RouteGroupBuilder builder) =>
        builder.MapDelete("journals/{id:guid}", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromRoute] Guid id,
        [FromQuery] string name,
        [FromQuery] string type,
        CancellationToken ct
    )
    {
        try
        {
            RemoveScraperJournalCommand command = new(id, name, type);
            await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
            RemoveScraperJournalHandler handler = new(dataSource, logger);
            Guid result = await handler.Handle(command, ct);
            return Results.Ok(result);
        }
        catch (CannotRemoveNotCompletedScraperJournalException ex)
        {
            return Results.BadRequest(new { messsage = ex.Message });
        }
        catch (ScraperJournalByIdNotFoundException ex)
        {
            return Results.NotFound(new { messsage = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Entrance}. {Ex}.", nameof(RemoveScraperJournalEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
