using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Features.CreateNewParserLink.Cache;
using Scrapers.Module.Features.CreateNewParserLink.Database;
using Scrapers.Module.Features.CreateNewParserLink.Exceptions;
using Scrapers.Module.Features.CreateNewParserLink.Logging;
using Scrapers.Module.Features.CreateNewParserLink.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.CreateNewParserLink.Endpoint;

public static class CreateNewParserLinkEndpoint
{
    public sealed record CreateNewParserLinkRequest(string Name, string Url);

    public sealed record CreateNewParserLinkResponse(
        string Name,
        string Url,
        string ParserName,
        string ParserType,
        int ProcessedAmount,
        long TotalElapsedSeconds,
        int ElapsedHours,
        int ElapsedMinutes,
        int ElapsedSeconds,
        bool Activity
    );

    public static void Map(RouteGroupBuilder builder) => builder.MapPost("scraper-link", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string name,
        [FromQuery] string type,
        [FromBody] CreateNewParserLinkRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IParsersWithNewLinkStorage storage = new LoggingParsersWithLinkStorage(
                logger,
                new NpgSqlParsersWithLinkStorage(dataSource)
            );
            ParserWhereToPutLink parser = await storage.Fetch(name, type, cancellationToken);
            ParserWithNewLink withLink = parser.Put(request.Name, request.Url);
            ParserWithNewLink saved = await storage.Save(withLink, cancellationToken);
            return Results.Ok(
                new CreateNewParserLinkResponse(
                    saved.Link.Name,
                    saved.Link.Url,
                    saved.Link.ParserName,
                    saved.Link.ParserType,
                    saved.Link.Statistics.ParsedAmount,
                    saved.Link.Statistics.TotalElapsedSeconds,
                    saved.Link.Statistics.ElapsedHours,
                    saved.Link.Statistics.ElapsedMinutes,
                    saved.Link.Statistics.ElapsedSeconds,
                    saved.Link.Active
                )
            );
        }
        catch (CannotPutLinkToWorkingParserException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (ParserLinkAlreadyExistsInParserException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
        catch (ParserLinkUrlAlreadyExistsException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
        catch (ParserWhereToPutLinkNameEmptyException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (ParserWhereToPutLinkNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (ParserWhereToPutLinkUnsupportedTypeException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnkownParserStateException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (ParserLinkDomainDoesntMatchException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }
}
