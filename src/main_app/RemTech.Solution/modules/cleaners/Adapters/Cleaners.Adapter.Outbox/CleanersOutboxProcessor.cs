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
            try
            {
                await DoLogic();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.Fatal("{Context}. Exception: {ExceptionMessage}.", Context, ex);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task DoLogic()
    {
        int processedMessages = 0;

        await using var channel = await provider.ProvideChannel();
        using var dbConnection = await database.ProvideConnection();
        using var transaction = dbConnection.BeginTransaction();

        try
        {
            // получение сообщения.
            var message = await GetMessage(dbConnection, transaction, channel);
            if (message is null)
            {
                // если нет сообщения - простой.
                logger.Information("{Context} no messages found.", Context);
                await Task.Delay(TimeSpan.FromSeconds(5));
                return;
            }

            try
            {
                // публикация сообщения.
                await message.Publish();
                processedMessages++;
            }
            catch (Exception ex)
            {
                // откат опубликации сообщения.
                await message.UpdateAsNotProcessed(ex.Message);
            }

            logger.Information(
                "{Context} Processing {Count} outbox messages.",
                Context,
                processedMessages
            );
        }
        catch (Exception ex)
        {
            logger.Error("{Context}. Error: {Exception}.", Context, ex.Message);
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