using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;
using Scrapers.Module.Features.ReadConcreteScraper.Storage;

namespace Scrapers.Module.Features.ReadConcreteScraper.Endpoint;

public static class ConcreteScraperEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet("{name}/{type}", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromRoute] string name,
        [FromRoute] string type,
        CancellationToken ct
    )
    {
        IConcreteScraperStorage storage = new NpgSqlConcreteScraperStorage(dataSource);
        ParserResult? parser = await storage.Read(name, type, ct);
        return parser != null
            ? Results.Ok(parser)
            : Results.NotFound(new { message = $"Парсер с {name} {type} не найден." });
    }
}
