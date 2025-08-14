using Cleaners.Module.Endpoints.Responses;
using Cleaners.Module.Services.Features.Common;
using Cleaners.Module.Services.Features.Enable;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace Cleaners.Module.Endpoints;

public static class EnableEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("enabled", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        CancellationToken ct
    )
    {
        NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        return new CleanerHttpResponseVeil(() =>
            new ResourceDisposingHandler<EnableCleaner>(
                new LoggingCleanerHandler<EnableCleaner>(
                    logger,
                    new SavingCleanerHandler<EnableCleaner>(
                        connection,
                        new EnableCleanerHandler(connection, logger)
                    )
                )
            )
                .With(connection)
                .Handle(new EnableCleaner(), ct)
        );
    }
}
