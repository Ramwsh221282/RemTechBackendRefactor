using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Features.ReadAllTransportParsers.Storage;
using StackExchange.Redis;

namespace Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;

internal static class ReadAllTransportParsersEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionMultiplexer multiplexer,
        CancellationToken ct
    )
    {
        IAllTransportParsersStorage storage = new CachedAllTransportParsersStorage(
            multiplexer,
            new NpgSqlAllTransportParsersStorage(dataSource)
        );
        IEnumerable<ParserResult> result = await storage.Read(ct);
        return Results.Ok(result);
    }
}
