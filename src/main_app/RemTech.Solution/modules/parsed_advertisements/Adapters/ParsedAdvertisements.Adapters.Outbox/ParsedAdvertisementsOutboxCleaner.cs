using Dapper;
using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;
using RemTech.Core.Shared.Transactions;

namespace ParsedAdvertisements.Adapters.Outbox;

public sealed class ParsedAdvertisementsOutboxCleaner : IParsedAdvertisementsOutboxCleaner
{
    private const string sql = """
                               DELETE FROM parsed_advertisements_module.outbox
                               WHERE 
                                   EXTRACT(day from NOW()::timestamp - created) > 1 AND
                                   (status = @status OR retries > 5)  
                               """;

    public async Task Remove(ITransactionManager transaction, CancellationToken ct = default)
    {
        var command = new CommandDefinition();
        var parameters = new { status = ParsedAdvertisementsOutboxMessage.Success };
        transaction.AccessConnection((_, txn) =>
        {
            command = new CommandDefinition(sql, parameters, transaction: txn);
        });

        await transaction.Execute(conn => conn.ExecuteAsync(command));
    }
}