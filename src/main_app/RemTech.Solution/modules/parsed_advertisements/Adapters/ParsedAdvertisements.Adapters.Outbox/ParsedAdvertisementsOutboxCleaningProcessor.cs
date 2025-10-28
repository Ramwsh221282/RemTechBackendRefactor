using ParsedAdvertisements.Domain.VehicleContext.Ports.Outbox;
using Quartz;
using Shared.Infrastructure.Module.Postgres;

namespace ParsedAdvertisements.Adapters.Outbox;

public sealed class ParsedAdvertisementsOutboxCleaningProcessor(
    IParsedAdvertisementsOutboxCleaner cleaner,
    PostgresDatabase database,
    Serilog.ILogger logger) : IJob
{
    private const string Context = nameof(ParsedAdvertisementsOutboxCleaningProcessor);

    public async Task Execute(IJobExecutionContext context)
    {
        logger.Information("{Context}. Processing messages...", Context);
        await using var transaction = database.ProvideTransactionManager();
        await transaction.Begin();

        try
        {
            await cleaner.Remove(transaction);
            var status = await transaction.Commit();
            if (status.IsFailure)
            {
                logger.Information("{Context}. Failed at cleaning messages. Error: {Error}.", Context,
                    status.Error.ErrorText);
            }
        }
        catch (Exception ex)
        {
            logger.Information("{Context}. Failed at cleaning messages. Error: {Error}.", Context, ex);
        }

        logger.Information("{Context}. Messages processed", Context);
    }
}