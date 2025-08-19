using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RabbitMQ.Client;
using Scrapers.Module.Domain.JournalsContext;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Domain.JournalsContext.Features.CreateScraperJournal;
using Scrapers.Module.Features.InstantlyEnableParser.Exceptions;
using Scrapers.Module.Features.InstantlyEnableParser.Models;
using Scrapers.Module.Features.InstantlyEnableParser.Storage;
using Scrapers.Module.Features.StartParser.RabbitMq;
using Scrapers.Module.ParserStateCache;

namespace Scrapers.Module.Features.InstantlyEnableParser.Endpoint;

public static class InstantlyEnableParserEndpoint
{
    public sealed record InstantlyEnabledParserResponse(
        string Name,
        string Type,
        string State,
        DateTime LastRun,
        DateTime NextRun
    );

    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("instant", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromServices] ConnectionFactory factory,
        [FromServices] ParserStateCachedStorage cache,
        [FromServices] ActiveScraperJournalsCache journalsCache,
        [FromQuery] string name,
        [FromQuery] string type,
        CancellationToken ct
    )
    {
        try
        {
            IInstantlyEnabledParsersStorage storage = new NpgSqlInstantlyEnabledParsersStorage(
                dataSource
            );
            ParserToInstantlyEnable toEnable = await storage.Fetch(name, type, ct);
            InstantlyEnabledParser enabled = toEnable.Enable();
            await enabled.Save(storage, ct);
            await cache.UpdateState(name, type, "Работает");
            await new CreateScraperJournalHandler(dataSource, logger, journalsCache).Handle(
                enabled.CreateJournalCommand(),
                ct
            );
            await using IParserStartedPublisher publisher = new RabbitMqParserStartedPublisher(
                factory
            );
            await toEnable.PublishStarted(publisher);
            enabled.LogMessage().Log(logger);
            return Results.Ok(enabled.CreateResponse());
        }
        catch (ParserToInstantlyEnableIsWorkingException ex)
        {
            logger.Error("{Error}", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (ParserToInstantlyEnableNotFoundException ex)
        {
            logger.Error("{Error}", ex.Message);
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UnableToInstantlyStartParserWithoutLinksException ex)
        {
            logger.Error("{Error}", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "Fatal {Entrance}. {Ex}.",
                nameof(InstantlyEnableParserEndpoint),
                ex.Message
            );
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения" });
        }
    }
}
