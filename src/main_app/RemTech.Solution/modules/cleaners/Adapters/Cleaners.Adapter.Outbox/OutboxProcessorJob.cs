using System.Data;
using Cleaners.Domain.Cleaners.Ports.Outbox;
using Dapper;
using Quartz;
using RabbitMQ.Client;
using Shared.Infrastructure.Module.Consumers;
using Shared.Infrastructure.Module.Postgres;

namespace Cleaners.Adapter.Outbox;

[DisallowConcurrentExecution]
public sealed class OutboxProcessorJob(
    PostgresDatabase database,
    RabbitMqConnectionProvider rabbit,
    Serilog.ILogger logger
) : IJob
{
    private const string Context = nameof(OutboxProcessorJob);

    public async Task Execute(IJobExecutionContext context)
    {
        logger.Information("{Context} processing job.", Context);
        try
        {
            await DoLogic();
        }
        catch (Exception ex)
        {
            logger.Error("{Context}. Error message: {Message}.", Context, ex);
        }
    }

    private async Task DoLogic()
    {
        int processedMessages = 0;

        await using var channel = await rabbit.ProvideChannel();
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
                return;
            }

            try
            {
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
