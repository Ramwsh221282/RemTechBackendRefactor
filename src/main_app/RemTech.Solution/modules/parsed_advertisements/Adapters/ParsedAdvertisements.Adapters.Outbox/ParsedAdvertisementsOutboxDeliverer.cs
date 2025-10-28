using Dapper;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Adapters.Outbox;

public sealed class ParsedAdvertisementsOutboxDeliverer : IParsedAdvertisementsOutboxDeliverer
{
    private const string sql = """
                               INSERT INTO parsed_advertisements_module.outbox
                               (id, content, created, last_error, retries, type, status)
                               VALUES
                               (@id, @content, @created, @last_error, @retries, @type, @status)                                   
                               """;

    public async Task<Status> Save(ParsedAdvertisementsOutboxMessage message, ITransactionManager transaction,
        CancellationToken ct = default)
    {
        var command = new CommandDefinition();

        var parameters = new
        {
            id = message.Id,
            content = message.Content,
            created = message.Created,
            last_error = message.LastError,
            retries = message.Retries,
            type = message.Type,
            status = message.Status
        };

        transaction.AccessConnection((_, txn) =>
        {
            command = new CommandDefinition(sql, parameters, cancellationToken: ct, transaction: txn);
        });

        await transaction.Execute(conn => conn.ExecuteAsync(command));
        return Status.Success();
    }
}