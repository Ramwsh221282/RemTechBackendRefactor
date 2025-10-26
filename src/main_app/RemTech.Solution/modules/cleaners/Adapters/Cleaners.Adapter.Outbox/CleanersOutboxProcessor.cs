using System.Data;
using Cleaners.Domain.Cleaners.Ports.Outbox;
using Dapper;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Serilog;
using Shared.Infrastructure.Module.Consumers;
using Shared.Infrastructure.Module.Postgres;

namespace Cleaners.Adapter.Outbox;

public sealed class CleanersOutboxProcessor(
    PostgresDatabase database,
    ILogger logger,
    RabbitMqConnectionProvider provider
) : BackgroundService
{
    private const string Context = nameof(CleanersOutboxProcessor);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            int processedMessages = 0;

            var connection = await provider.ProvideConnection(stoppingToken);
            await using var channel = await connection.CreateChannelAsync(
                cancellationToken: stoppingToken
            );

            using var dbConnection = await database.ProvideConnection(ct: stoppingToken);
            using var transaction = dbConnection.BeginTransaction();

            try
            {
                var message = await GetMessage(dbConnection, transaction, channel);
                if (message is null)
                {
                    logger.Information("{Context} no messages found.", Context);
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    continue;
                }

                try
                {
                    await message.Publish();
                    processedMessages++;
                }
                catch (Exception ex)
                {
                    await message.UpdateAsNotProcessed(ex.Message);
                }

                logger.Information(
                    "{Context} Processing {Count} outbox messages.",
                    Context,
                    processedMessages
                );
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.Error("{Context}. Error: {Exception}.", Context, ex.Message);
            }
        }
    }

    private async Task<TransactionalOutboxMessage?> GetMessage(
        IDbConnection connection,
        IDbTransaction transaction,
        IChannel channel
    )
    {
        const string sql = """
            SELECT
            id,
            type,
            content,
            created,
            processed,
            processed_attempts,
            last_error
            FROM cleaners_module.outbox
            WHERE processed IS NULL AND processed_attempts < 20
            LIMIT 1
            FOR UPDATE SKIP LOCKED
            """;

        var command = new CommandDefinition(sql, transaction: transaction);
        var message = await connection.QueryFirstOrDefaultAsync<CleanerOutboxMessage>(command);

        return message == null
            ? null
            : new TransactionalOutboxMessage(message, channel, transaction, connection);
    }
}
