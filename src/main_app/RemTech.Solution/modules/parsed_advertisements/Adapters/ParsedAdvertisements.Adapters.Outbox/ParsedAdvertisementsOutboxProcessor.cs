using System.Text.Json;
using Dapper;
using ParsedAdvertisements.Domain.VehicleContext.Events;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Messaging;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;
using Quartz;
using RemTech.Core.Shared.Transactions;
using Shared.Infrastructure.Module.Postgres;

namespace ParsedAdvertisements.Adapters.Outbox;

public sealed class ParsedAdvertisementsOutboxProcessor(
    Serilog.ILogger logger,
    PostgresDatabase database,
    IVehicleCreatedEventPublisher publisher) : IJob
{
    private const string Context = nameof(ParsedAdvertisementsOutboxProcessor);

    public async Task Execute(IJobExecutionContext context)
    {
        logger.Information("{Context}. Processing messages.", Context);
        await using var manager = database.ProvideTransactionManager();
        await manager.Begin();

        var requiredType = nameof(VehicleCreatedEvent);
        var messages = (await GetMessages(manager, requiredType)).ToArray();

        await ProcessMessages(messages, manager);
        var committing = await manager.Commit();

        if (committing.IsFailure)
        {
            logger.Error("{Context}. Failed to commit message changes. Error: {Error}.", Context,
                committing.Error.ErrorText);
        }
        else
        {
            logger.Error("{Context}. Processed {Count} messages.", Context, messages.Length);
        }
    }

    private async Task ProcessMessages(
        IEnumerable<ParsedAdvertisementsOutboxMessage> messages,
        ITransactionManager transaction)
    {
        foreach (var message in messages)
        {
            try
            {
                var msg = await HandleMessage(message);
                msg.Status = ParsedAdvertisementsOutboxMessage.Success;
                msg.LastError = null;
                await UpdateMessage(msg, transaction);
            }
            catch (Exception ex)
            {
                message.Status = ParsedAdvertisementsOutboxMessage.Pending;
                message.LastError = ex.Message;
                message.Retries += 1;
                await UpdateMessage(message, transaction);
                logger.Error("{Context}. Failed publish message: {ID}. Error: {Error}",
                    Context, message.Id, ex.Message);
            }
        }
    }

    private async Task UpdateMessage(ParsedAdvertisementsOutboxMessage message, ITransactionManager transaction)
    {
        const string sql = """
                           UPDATE parsed_advertisements_module.outbox
                           SET status = @status,
                               last_error = @error,
                               retries = @retries
                           WHERE id = @id
                           """;

        var command = new CommandDefinition();
        transaction.AccessConnection((_, txn) =>
        {
            command = new CommandDefinition(sql, new
            {
                status = message.Status,
                last_error = message.LastError,
                retries = message.Retries,
                id = message.Id
            }, transaction: txn);
        });

        await transaction.Execute(conn => conn.ExecuteAsync(command));
    }

    private async Task<ParsedAdvertisementsOutboxMessage> HandleMessage(ParsedAdvertisementsOutboxMessage message)
    {
        VehicleCreatedEvent? @event = JsonSerializer.Deserialize<VehicleCreatedEvent>(message.Content);
        if (@event == null)
            return message;
        await publisher.Publish(@event);
        return message;
    }

    private async Task<IEnumerable<ParsedAdvertisementsOutboxMessage>> GetMessages(ITransactionManager manager,
        string type)
    {
        const string sql = """
                           SELECT 
                               id, 
                               content,
                               created,
                               last_error,
                               retries,
                               type,
                               status
                           FROM parsed_advertisements_module.outbox
                           WHERE status = @status AND type = @type
                           FOR UPDATE SKIP LOCKED
                           """;

        var status = ParsedAdvertisementsOutboxMessage.Pending;
        var parameters = new { status, type };
        var command = new CommandDefinition();

        manager.AccessConnection((_, txn) => { command = new CommandDefinition(sql, parameters, transaction: txn); });
        return await manager.Execute(conn => conn.QueryAsync<ParsedAdvertisementsOutboxMessage>(command));
    }
}