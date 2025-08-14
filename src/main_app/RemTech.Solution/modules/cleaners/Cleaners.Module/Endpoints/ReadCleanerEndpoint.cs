using Cleaners.Module.Database;
using Cleaners.Module.Domain;
using Cleaners.Module.Endpoints.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace Cleaners.Module.Endpoints;

public static class ReadCleanerEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapGet(string.Empty, Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        CancellationToken ct
    )
    {
        NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        ICleaners cleaners = new NpgSqlCleaners(connection);
        IResult result = new CleanerHttpResponseVeil(() => cleaners.Single(ct));
        return new ResourceDisposingHttpResult(result).With(connection);
    }
}
