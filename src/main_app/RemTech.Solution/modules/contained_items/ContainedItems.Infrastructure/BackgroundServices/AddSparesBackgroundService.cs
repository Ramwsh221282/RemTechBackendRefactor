using ContainedItems.Domain.Contracts;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.Producers;
using ContainedItems.Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ContainedItems.Infrastructure.BackgroundServices;

public sealed class AddSparesBackgroundService(
    Serilog.ILogger logger, 
    NpgSqlConnectionFactory connectionFactory,
    AddSparesProducer producer) 
    : BackgroundService
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<AddSparesBackgroundService>();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InvokePublishing(stoppingToken);
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
    }

    private async Task InvokePublishing(CancellationToken ct)
    {
        Logger.Information("Invoking publishing");
        try
        {
            // await using NpgSqlSession session = new(connectionFactory);
            // ITransactionSource source = new NpgSqlTransactionSource(session);
            // ITransactionScope transaction = await source.BeginTransaction(ct);
            // IContainedItemsRepository repository = new ContainedItemsRepository(session);
            // ContainedItemsQuery query = new(Status: ContainedItemStatus.PendingToSave.Value);
            // ContainedItem[] items = await repository.Query(query, ct);
            // if (items.Length == 0) return;
            // await producer.Publish(items, ct);
            // Logger.Information("Published {Count}", items.Length);
            //
            // foreach (ContainedItem item in items)
            //     item.MarkSaved();
            //
            // await repository.UpdateMany(items, ct);
            // Result committing = await transaction.Commit(ct);
            // if (committing.IsFailure)
            // {
            //     Logger.Fatal(committing.Error, "Error committing transaction.");
            // }
            // else
            // {
            //     Logger.Information("Committed {Count}", items.Length);
            // }
        }
        catch(Exception e)
        {
            Logger.Fatal(e, "Error publishing add spares message.");
        }
    }
}