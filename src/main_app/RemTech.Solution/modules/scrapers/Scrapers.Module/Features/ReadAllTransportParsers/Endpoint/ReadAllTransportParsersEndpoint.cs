using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using Scrapers.Module.Features.ReadAllTransportParsers.Storage;

namespace Scrapers.Module.Features.ReadAllTransportParsers.Endpoint;

public static class ReadAllTransportParsersEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        CancellationToken ct
    )
    {
        IAllTransportParsersStorage storage = new NpgSqlAllTransportParsersStorage(dataSource);
        IEnumerable<ParserResult> result = await storage.Read(ct);
        return Results.Ok(result);
    }
}
