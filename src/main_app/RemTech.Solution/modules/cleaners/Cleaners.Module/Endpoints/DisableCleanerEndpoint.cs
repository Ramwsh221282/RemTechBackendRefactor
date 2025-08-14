using Cleaners.Module.Endpoints.Responses;
using Cleaners.Module.Services.Features.Common;
using Cleaners.Module.Services.Features.DisableCleaner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace Cleaners.Module.Endpoints;

public static class DisableCleanerEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("disabled", Handle);

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] NpgsqlDataSource dataSource,
        CancellationToken ct
    )
    {
        NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        return new CleanerHttpResponseVeil(() =>
            new ResourceDisposingHandler<DisableCleanerCommand>(
                new LoggingCleanerHandler<DisableCleanerCommand>(
                    logger,
                    new SavingCleanerHandler<DisableCleanerCommand>(
                        connection,
                        new DisableCleanerHandler(connection, logger)
                    )
                )
            )
                .With(connection)
                .Handle(new DisableCleanerCommand(), ct)
        );
    }
}
