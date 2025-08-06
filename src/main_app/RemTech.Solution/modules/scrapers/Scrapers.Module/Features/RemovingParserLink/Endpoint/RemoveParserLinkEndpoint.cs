using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Features.RemovingParserLink.Database;
using Scrapers.Module.Features.RemovingParserLink.Exceptions;
using Scrapers.Module.Features.RemovingParserLink.Logging;
using Scrapers.Module.Features.RemovingParserLink.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.RemovingParserLink.Endpoint;

internal static class RemoveParserLinkEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapDelete("scraper-link", Handle);

    public sealed record RemoveParserLinkRequest(string LinkName);

    public sealed record RemoveParserLinkResponse(
        string ParserName,
        string ParserType,
        string LinkName,
        string LinkUrl
    );

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string name,
        [FromQuery] string type,
        [FromBody] RemoveParserLinkRequest request,
        CancellationToken ct
    )
    {
        try
        {
            IRemovedParserLinksStorage storage = new LoggingRemovedParserLinkStorage(
                logger,
                new NpgSqlLinksToRemoveStorage(dataSource)
            );
            ParserLinkToRemove toRemove = await storage.Fetch(request.LinkName, name, type, ct);
            RemovedParserLink removed = toRemove.Remove();
            RemovedParserLink saved = await storage.Save(removed, ct);
            return Results.Ok(
                new RemoveParserLinkResponse(
                    saved.ParserName,
                    saved.ParserType,
                    saved.Name,
                    saved.Url
                )
            );
        }
        catch (ParserLinkToRemoveNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UnableToRemoveParserLinkWhenWorkingException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
