using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Domain.JournalsContext.Exceptions;
using Scrapers.Module.Domain.JournalsContext.Features.CompleteScraperJournal;
using Scrapers.Module.Domain.JournalsContext.Persistance;
using Scrapers.Module.Features.ChangeParserState.Database;
using Scrapers.Module.Features.ChangeParserState.Exception;
using Scrapers.Module.Features.ChangeParserState.Logging;
using Scrapers.Module.Features.ChangeParserState.Models;
using Scrapers.Module.ParserStateCache;

namespace Scrapers.Module.Features.ChangeParserState.Endpoint;

public static class ParserStateChangeEndpoint
{
    public sealed record ParserStateToChangeRequest(bool StateSwitch);

    public sealed record ParserStateChangedResult(
        string ParserName,
        string ParserType,
        string NewState
    );

    public static void Map(RouteGroupBuilder group) => group.MapPatch("state", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] ParserStateCachedStorage cache,
        [FromServices] ActiveScraperJournalsCache journalsCache,
        [FromQuery] string name,
        [FromQuery] string type,
        [FromBody] ParserStateToChangeRequest request,
        CancellationToken ct
    )
    {
        try
        {
            IParserStateToChangeStorage storage = new LoggingParserStateStorage(
                logger,
                new NpgSqlParserStateToChange(dataSource)
            );
            ParserStateToChange parser = await storage.Fetch(name, type, ct);
            ParserWithChangedState changed = parser.Change(request.StateSwitch);
            ParserWithChangedState saved = await storage.Save(changed, ct);
            ParserStateChangedResult result = new ParserStateChangedResult(
                saved.ParserName,
                saved.ParserType,
                saved.NewState
            );
            await cache.UpdateState(saved.ParserName, saved.ParserType, saved.NewState);
            if (saved.NewState == "Отключен")
            {
                await new CompleteScraperJournalHandler(dataSource, logger, journalsCache).Handle(
                    new CompleteScraperJournalCommand(result.ParserName, result.ParserType),
                    ct
                );
            }
            return Results.Ok(result);
        }
        catch (ParserAlreadyHasThisStateException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (ParserStateToChangeNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (ScraperJournalByIdNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (System.Exception ex)
        {
            logger.Error("Fatal {Endpoint}. {Ex}.", nameof(ParserStateChangeEndpoint), ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
