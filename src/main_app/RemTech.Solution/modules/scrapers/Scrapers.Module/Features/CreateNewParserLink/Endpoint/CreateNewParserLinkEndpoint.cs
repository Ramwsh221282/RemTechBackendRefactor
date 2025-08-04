using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        int ProcessedAmount,
        long TotalElapsedSeconds,
        int ElapsedHours,
        int ElapsedMinutes,
        int ElapsedSeconds,
        bool Activity
    );

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] Serilog.ILogger logger,
        [FromQuery] string name,
        [FromQuery] string type,
        [FromQuery] string state,
        [FromBody] CreateNewParserLinkRequest request
    )
    {
        ParserWhereToPutLink parser = new(name, type, state);
        try
        {
            ParserWithNewLink withLink = await new LoggingParsersWithLinkStorage(
                logger,
                new CachedParsersWithLinkStorage(
                    multiplexer,
                    new NpgSqlParsersWithLinkStorage(dataSource)
                )
            ).Save(parser.Put(request.Name, request.Url));
            return Results.Ok(
                new CreateNewParserLinkResponse(
                    withLink.Link.Name,
                    withLink.Link.Url,
                    withLink.Link.ParserName,
                    withLink.Link.Statistics.ParsedAmount,
                    withLink.Link.Statistics.TotalElapsedSeconds,
                    withLink.Link.Statistics.ElapsedHours,
                    withLink.Link.Statistics.ElapsedMinutes,
                    withLink.Link.Statistics.ElapsedSeconds,
                    withLink.Link.Active
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
    }
}
