using Cleaners.Module.Endpoints.Responses;
using Cleaners.Module.Services.Features.ChangeItemsThreshold;
using Cleaners.Module.Services.Features.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace Cleaners.Module.Endpoints;

public static class ChangeCleanerThresholdEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("treshold", Handle);

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] NpgsqlDataSource dataSource,
        [FromBody] ChangeItemsThresholdCommand command,
        CancellationToken ct
    )
    {
        NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        return new CleanerHttpResponseVeil(() =>
            new ResourceDisposingHandler<ChangeItemsThresholdCommand>(
                new LoggingCleanerHandler<ChangeItemsThresholdCommand>(
                    logger,
                    new SavingCleanerHandler<ChangeItemsThresholdCommand>(
                        connection,
                        new ChangeItemsThresholdCommandHandler(connection, logger)
                    )
                )
            )
                .With(connection)
                .Handle(command, ct)
        );
    }
}
