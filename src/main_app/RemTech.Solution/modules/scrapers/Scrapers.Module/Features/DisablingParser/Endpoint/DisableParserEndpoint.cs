using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Scrapers.Module.Features.DisablingParser.Cache;
using Scrapers.Module.Features.DisablingParser.Database;
using Scrapers.Module.Features.DisablingParser.Exceptions;
using Scrapers.Module.Features.DisablingParser.Logging;
using Scrapers.Module.Features.DisablingParser.Models;
using StackExchange.Redis;

namespace Scrapers.Module.Features.DisablingParser.Endpoint;

public static class DisableParserEndpoint
{
    public sealed record DisableParserResult(string Name, string State);

    private static async Task<IResult> Handle(
        [FromQuery] string name,
        [FromQuery] string type,
        [FromQuery] string state,
        [FromServices] ConnectionMultiplexer multiplexer,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        ParserToDisable toDisable = new(name, type, state);
        try
        {
            DisabledParser disabled = await new LoggingDisabledParsersStorage(
                logger,
                new CacheDisabledParsersStorage(
                    multiplexer,
                    new NpgSqlDisabledParsersStorage(dataSource)
                )
            ).SaveAsync(toDisable.Disable(), ct);
            return Results.Ok(new DisableParserResult(disabled.Name, disabled.State));
        }
        catch (UnableToDisableDisabledParserException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnableToDisableWorkingParserException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnableToFindParserToDisableException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (UnsupportedDisabledParserTypeException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
