using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using RabbitMQ.Client;
using Scrapers.Module.Features.EnableParser.Database;
using Scrapers.Module.Features.EnableParser.Exceptions;
using Scrapers.Module.Features.EnableParser.Logging;
using Scrapers.Module.Features.EnableParser.Models;
using Scrapers.Module.Features.EnableParser.RabbitMq;

namespace Scrapers.Module.Features.EnableParser.Endpoint;

public static class EnableParserEndpoint
{
    public sealed record EnabledParserResult(string Name, string State);

    private static async Task<IResult> Handle(
        [FromQuery] string name,
        [FromQuery] string state,
        [FromQuery] string type,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionFactory connectionFactory,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        ParserToEnable toEnable = new(name, state, type);
        try
        {
            EnabledParser enabled = await new LoggingEnabledParsersStorage(
                logger,
                new RabbitMqParsersStorage(
                    connectionFactory,
                    new PgEnabledParsersStorage(dataSource)
                )
            ).Save(toEnable.Enable(), ct);
            return Results.Ok(new EnabledParserResult(enabled.Name, enabled.State));
        }
        catch (ParserToEnableWasNotFoundException ex)
        {
            return Results.NotFound(ex.Message);
        }
        catch (UnableToEnableParserWhenWorkingException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnableToEnableWaitingParserException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (UnsupportedEnabledParserTypeException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("{Ex}.", ex.Message);
            return Results.InternalServerError(new { message = "Ошибка на стороне приложения." });
        }
    }
}
