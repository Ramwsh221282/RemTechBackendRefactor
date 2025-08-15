using Cleaners.Module.Cache;
using Cleaners.Module.Endpoints.Responses;
using Cleaners.Module.Services.Features.Common;
using Cleaners.Module.Services.Features.PermantlyEnable;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Npgsql;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace Cleaners.Module.Endpoints;

public static class PermantlyEnableCleanerEndpoint
{
    public static void Map(RouteGroupBuilder builder) => builder.MapPatch("instant", Handle);

    private static async Task<IResult> Handle(
        [FromServices] Serilog.ILogger logger,
        [FromServices] NpgsqlDataSource dataSource,
        [FromServices] ConnectionFactory rabbitMqFactory,
        [FromServices] ConnectionMultiplexer multiplexer,
        CancellationToken ct
    )
    {
        NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        return new CleanerHttpResponseVeil(() =>
            new ResourceDisposingHandler<PermantlyEnableCleanerCommand>(
                new LoggingCleanerHandler<PermantlyEnableCleanerCommand>(
                    logger,
                    new PermantlyEnableCleanerHandler(
                        connection,
                        logger,
                        rabbitMqFactory,
                        new CleanerStateCache(multiplexer)
                    )
                )
            )
                .With(connection)
                .Handle(new PermantlyEnableCleanerCommand(), ct)
        );
    }
}
