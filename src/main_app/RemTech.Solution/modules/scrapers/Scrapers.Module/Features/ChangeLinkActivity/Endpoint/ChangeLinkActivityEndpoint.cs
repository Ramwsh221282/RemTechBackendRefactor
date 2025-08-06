using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Scrapers.Module.Features.ChangeLinkActivity.Cache;
using Scrapers.Module.Features.ChangeLinkActivity.Database;
using Scrapers.Module.Features.ChangeLinkActivity.Exceptions;
using Scrapers.Module.Features.ChangeLinkActivity.Logging;
using Scrapers.Module.Features.ChangeLinkActivity.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.ChangeLinkActivity.Endpoint;

public static class ChangeLinkActivityEndpoint
{
    public sealed record LinkWithChangedActivityResult(
        string LinkName,
        string ParserName,
        string ParserType,
        bool CurrentActivity
    );

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource source,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string linkName,
        [FromQuery] string parserName,
        [FromQuery] string parserType,
        CancellationToken ct
    )
    {
        try
        {
            ILinkActivityToChangeStorage storage = new LoggingLinkActivityToChange(
                logger,
                new NpgSqlLinkActivityToChangeStorage(source)
            );
            LinkActivityToChange link = await storage.Fetch(linkName, parserName, parserType, ct);
            LinkWithChangedActivity changed = await storage.Save(link.Change(), ct);
            return Results.Ok(
                new LinkWithChangedActivityResult(
                    changed.Name,
                    changed.ParserName,
                    changed.ParserType,
                    changed.CurrentActivity
                )
            );
        }
        catch (LinkActivityToChangeNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UnableToChangeLinkActivityOfWorkingParserException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new { message = "Ошибка на уровне приложения" });
        }
    }
}
