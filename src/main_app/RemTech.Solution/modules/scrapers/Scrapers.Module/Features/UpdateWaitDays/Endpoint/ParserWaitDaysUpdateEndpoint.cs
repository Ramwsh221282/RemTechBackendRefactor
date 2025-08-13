using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Features.UpdateWaitDays.Database;
using Scrapers.Module.Features.UpdateWaitDays.Exceptions;
using Scrapers.Module.Features.UpdateWaitDays.Logging;
using Scrapers.Module.Features.UpdateWaitDays.Models;

namespace Scrapers.Module.Features.UpdateWaitDays.Endpoint;

public static class ParserWaitDaysUpdateEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("wait-days", Handle);

    public sealed record ParserWaitDaysUpdateRequest(int NewWaitDays);

    public sealed record ParserWaitDaysUpdateResult(
        string ParserName,
        string ParserType,
        int NewWaitDays,
        DateTime NextRun
    );

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string name,
        [FromQuery] string type,
        [FromBody] ParserWaitDaysUpdateRequest request,
        CancellationToken ct
    )
    {
        try
        {
            IParserWaitDaysToUpdateStorage storage = new LoggingParserWaitDaysToChangeStorage(
                logger,
                new NpgSqlParserWaitDaysToUpdateStorage(dataSource)
            );
            ParserWaitDaysToUpdate parser = await storage.Fetch(name, type, ct);
            ParserWithUpdatedWaitDays changed = parser.Update(request.NewWaitDays);
            ParserWithUpdatedWaitDays saved = await storage.Save(changed, ct);
            ParserWaitDaysUpdateResult result = new(
                saved.ParserName,
                saved.ParserType,
                saved.WaitDays,
                saved.NextRun
            );
            return Results.Ok(result);
        }
        catch (InvalidParserWaitDaysToUpdateException ex)
        {
            logger.Error("{Ex}", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (ParserToUpdateWaitDaysNotFoundException ex)
        {
            logger.Error("{Ex}", ex.Message);
            return Results.NotFound(new { message = ex.Message });
        }
        catch (ParserWaitDaysExceesMaxAmountException ex)
        {
            logger.Error("{Ex}", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnableToUpdateWaitDaysForWorkingParserException ex)
        {
            logger.Error("{Ex}", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (WaitDaysSameException ex)
        {
            logger.Error("{Ex}", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "Fatal {Ex} {Endpoint}.",
                ex.Message,
                nameof(ParserWaitDaysUpdateEndpoint)
            );
            return Results.InternalServerError(new { message = ex.Message });
        }
    }
}
