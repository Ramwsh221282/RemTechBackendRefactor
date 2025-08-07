using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Features.ChangeLinkActivity.Database;
using Scrapers.Module.Features.ChangeLinkActivity.Exceptions;
using Scrapers.Module.Features.ChangeLinkActivity.Logging;
using Scrapers.Module.Features.ChangeLinkActivity.Models;

namespace Scrapers.Module.Features.ChangeLinkActivity.Endpoint;

public static class ChangeLinkActivityEndpoint
{
    public sealed record LinkWithChangedActivityRequest(bool Activity);

    public sealed record LinkWithChangedActivityResponse(
        string LinkName,
        string ParserName,
        string ParserType,
        bool NewActivity
    );

    public static void Map(RouteGroupBuilder builder) =>
        builder.MapPatch("scraper-link/activity", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource source,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string linkName,
        [FromQuery] string parserName,
        [FromQuery] string parserType,
        [FromBody] LinkWithChangedActivityRequest request,
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
            LinkWithChangedActivity changed = link.Change(request.Activity);
            LinkWithChangedActivity saved = await storage.Save(changed, ct);
            LinkWithChangedActivityResponse response = new(
                saved.Name,
                saved.ParserName,
                saved.ParserType,
                saved.CurrentActivity
            );
            return Results.Ok(response);
        }
        catch (LinkActivityToChangeNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UnableToChangeLinkActivityOfWorkingParserException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (LinkActivitySameException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new { message = "Ошибка на уровне приложения" });
        }
    }
}
