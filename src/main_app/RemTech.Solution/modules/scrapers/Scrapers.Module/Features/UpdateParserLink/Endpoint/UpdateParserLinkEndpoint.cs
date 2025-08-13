using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Features.UpdateParserLink.Database;
using Scrapers.Module.Features.UpdateParserLink.Exceptions;
using Scrapers.Module.Features.UpdateParserLink.Models;

namespace Scrapers.Module.Features.UpdateParserLink.Endpoint;

public static class UpdateParserLinkEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPut("scraper-link", Handle);

    public sealed record UpdateParserLinkEndpointRequest(string LinkName, string LinkUrl);

    public sealed record UpdateParserLinkEndpointResponse(
        string ParserName,
        string ParserType,
        string LinkName,
        string LinkUrl
    );

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromQuery] string name,
        [FromQuery] string type,
        [FromQuery] string linkName,
        [FromQuery] string linkUrl,
        [FromBody] UpdateParserLinkEndpointRequest request,
        Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        try
        {
            IParserLinkToUpdateStorage storage = new NpgSqlParserLinkToUpdateStorage(dataSource);
            ParserLinkToUpdate parser = await storage.Fetch(name, type, linkName, linkUrl, ct);
            UpdatedParserLink updated = parser.Update(request.LinkUrl, request.LinkName);
            await updated.Save(storage, ct);
            updated.LogMessage().Log(logger);
            return Results.Ok(
                new UpdateParserLinkEndpointResponse(name, type, request.LinkName, request.LinkUrl)
            );
        }
        catch (ParserLinkToUpdateNotFoundException ex)
        {
            logger.Error("{Ex}.", ex.Message);
            return Results.NotFound(new { message = ex.Message });
        }
        catch (ParserLinkUpdateDomainMisatchException ex)
        {
            logger.Error("{Ex}.", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (ParserLinkUpdateLinkUrlException ex)
        {
            logger.Error("{Ex}.", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (ParserLinkUpdateNameEmptyException ex)
        {
            logger.Error("{Ex}.", ex.Message);
            return Results.BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.Fatal("Fatal {Endpoint}. {Ex}.", nameof(UpdateParserLinkEndpoint), ex.Message);
            return Results.InternalServerError(new { message = ex.Message });
        }
    }
}
