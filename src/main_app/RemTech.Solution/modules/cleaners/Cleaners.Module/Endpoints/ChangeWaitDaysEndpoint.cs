using Cleaners.Module.Endpoints.Responses;
using Cleaners.Module.Services.Features.ChangeWaitDays;
using Cleaners.Module.Services.Features.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;

namespace Cleaners.Module.Endpoints;

public static class ChangeWaitDaysEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("wait-days", Handle);

    private static async Task<IResult> Handle(
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] Serilog.ILogger logger,
        [FromBody] ChangeCleanerWaitDays command,
        CancellationToken ct
    )
    {
        NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        return new CleanerHttpResponseVeil(() =>
            new ResourceDisposingHandler<ChangeCleanerWaitDays>(
                new LoggingCleanerHandler<ChangeCleanerWaitDays>(
                    logger,
                    new SavingCleanerHandler<ChangeCleanerWaitDays>(
                        connection,
                        new ChangeCleanerWaitDaysHandler(connection, logger)
                    )
                )
            )
                .With(connection)
                .Handle(command, ct)
        );
    }
}
