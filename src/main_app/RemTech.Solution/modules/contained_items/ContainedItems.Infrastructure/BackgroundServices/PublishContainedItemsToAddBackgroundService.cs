using ContainedItems.Domain.Contracts;
using ContainedItems.Domain.Models;
using ContainedItems.Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace ContainedItems.Infrastructure.BackgroundServices;

/// <summary>
/// Фоновая служба публикации содержащихся элементов для добавления.
/// </summary>
/// <param name="logger">Логгер для записи информации и ошибок.</param>
/// <param name="connectionFactory">Фабрика для создания подключений к базе данных.</param>
/// <param name="factory">Фабрика стратегий публикации содержащихся элементов.</param>
public sealed class PublishContainedItemsToAddBackgroundService(
	Serilog.ILogger logger,
	NpgSqlConnectionFactory connectionFactory,
	ItemPublishStrategyFactory factory
) : BackgroundService
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<PublishContainedItemsToAddBackgroundService>();

	/// <summary>
	/// Основной метод выполнения фоновой службы.
	/// </summary>
	/// <param name="stoppingToken">Токен отмены для остановки фоновой службы.</param>
	/// <returns>Задача, представляющая асинхронную операцию выполнения фоновой службы.</returns>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await InvokePublishing(stoppingToken);
			await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
		}
	}

	private static async Task PublishItems(
		IEnumerable<ContainedItem> items,
		ItemPublishStrategyFactory factory,
		CancellationToken ct
	)
	{
		foreach (IGrouping<string, ContainedItem> group in items.GroupBy(i => i.CreatorInfo.Type))
		{
			string type = group.Key;
			ContainedItem[] itemsOfType = [.. group];
			IItemPublishingStrategy strategy = factory.Resolve(type);
			await strategy.PublishMany(itemsOfType, ct);
		}
	}

	private static async Task<IEnumerable<ContainedItem>> GetPendingContainedItems(
		NpgSqlSession session,
		CancellationToken ct
	)
	{
		ContainedItemsRepository repository = new(session);
		ContainedItemsQuery query = new(Status: ContainedItemStatus.PendingToSave.Value, Limit: 50, WithLock: true);
		return await repository.Query(query, ct);
	}

	private static Task MarkItemsSaved(NpgSqlSession session, IEnumerable<ContainedItem> items, CancellationToken ct)
	{
		ContainedItemsRepository repository = new(session);
		foreach (ContainedItem item in items)
			item.MarkSaved();
		return repository.UpdateMany(items, ct);
	}

	private async Task InvokePublishing(CancellationToken ct)
	{
		Logger.Information("Invoking publishing");
		await using NpgSqlSession session = new(connectionFactory);
		NpgSqlTransactionSource source = new(session);
		ITransactionScope transaction = await source.BeginTransaction(ct);

		try
		{
			IEnumerable<ContainedItem> items = await GetPendingContainedItems(session, ct);
			if (!items.Any())
			{
				return;
			}

			await PublishItems(items, factory, ct);
			await MarkItemsSaved(session, items, ct);
			Result committing = await transaction.Commit(ct);

			if (committing.IsFailure)
			{
				Logger.Fatal(committing.Error, "Error committing transaction.");
			}
			else
			{
				Logger.Information("Committed transaction");
			}
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
}
