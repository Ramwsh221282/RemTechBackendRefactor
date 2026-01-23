using ContainedItems.Domain.Contracts;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ContainedItems.Infrastructure.BackgroundServices;

public sealed class PublishContainedItemsToAddBackgroundService(
	Serilog.ILogger logger,
	NpgSqlConnectionFactory connectionFactory,
	ItemPublishStrategyFactory factory
) : BackgroundService
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<PublishContainedItemsToAddBackgroundService>();

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await InvokePublishing(stoppingToken);
			await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
		}
	}

	private async Task InvokePublishing(CancellationToken ct)
	{
		Logger.Information("Invoking publishing");
		await using NpgSqlSession session = new(connectionFactory);
		ITransactionSource source = new NpgSqlTransactionSource(session);
		ITransactionScope transaction = await source.BeginTransaction(ct);

		try
		{
			IEnumerable<ContainedItem> items = await GetPendingContainedItems(session, ct);
			if (!items.Any())
				return;
			await PublishItems(items, factory, ct);
			await MarkItemsSaved(session, items, ct);
			Result committing = await transaction.Commit(ct);
			if (committing.IsFailure)
				Logger.Fatal(committing.Error, "Error committing transaction.");
			else
				Logger.Information("Committed transaction");
		}
		catch (Exception e)
		{
			Logger.Fatal(e, "Error publishing add spares message.");
		}
		finally
		{
			await transaction.DisposeAsync();
		}
	}

	private static async Task PublishItems(
		IEnumerable<ContainedItem> items,
		ItemPublishStrategyFactory factory,
		CancellationToken ct
	)
	{
		IGrouping<string, ContainedItem>[] itemsGroupedByType = items.GroupBy(i => i.CreatorInfo.Type).ToArray();
		foreach (IGrouping<string, ContainedItem> group in itemsGroupedByType)
		{
			string type = group.Key;
			ContainedItem[] itemsOfType = group.ToArray();
			IItemPublishingStrategy strategy = factory.Resolve(type);
			await strategy.PublishMany(itemsOfType, ct);
		}
	}

	private static async Task<IEnumerable<ContainedItem>> GetPendingContainedItems(
		NpgSqlSession session,
		CancellationToken ct
	)
	{
		IContainedItemsRepository repository = new ContainedItemsRepository(session);
		ContainedItemsQuery query = new(Status: ContainedItemStatus.PendingToSave.Value, WithLock: true, Limit: 50);
		return await repository.Query(query, ct);
	}

	private static async Task MarkItemsSaved(
		NpgSqlSession session,
		IEnumerable<ContainedItem> items,
		CancellationToken ct
	)
	{
		IContainedItemsRepository repository = new ContainedItemsRepository(session);
		foreach (ContainedItem item in items)
			item.MarkSaved();
		await repository.UpdateMany(items, ct);
	}
}
