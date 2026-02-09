using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Configurations;
using Serilog;
using StackExchange.Redis;
using Telemetry.Core.ActionRecords;
using Telemetry.Core.ActionRecords.ValueObjects;

namespace Telemetry.Infrastructure;

/// <summary>
/// Запись действий телеметрии в Redis.
/// </summary>
public sealed class RedisTelemetryActionsStorage : IDisposable, IAsyncDisposable
{
	private const string ACTION_RECORDS_ARRAY_KEY = "Telemetry:ActionRecords";
	private readonly ConcurrentQueue<ActionRecord> _recordsQueue = [];
	private ConnectionMultiplexer? _multiplexer { get; set; }
	private readonly CancellationTokenSource _timerCts = new();
	private readonly Task _runningPeriodicallyWritingTask;
	private readonly SemaphoreSlim _semaphore = new(1, 1);

	/// <summary>
	/// Создает новый экземпляр <see cref="RedisTelemetryActionsStorage"/>.
	/// </summary>
	/// <param name="options">Опции кэширования.</param>
	/// <param name="logger">Логгер для записи логов.</param>
	public RedisTelemetryActionsStorage(IOptions<CachingOptions> options, ILogger logger)
	{
		Logger = logger;
		Options = options.Value;
		_runningPeriodicallyWritingTask = Task.Run(() => RunPeriodicProcessingAsync(_timerCts.Token));
	}

	private ILogger Logger { get; }
	private IDatabase? _database { get; set; }
	private CachingOptions Options { get; }

	/// <summary>
	/// Записывает запись действия.
	/// </summary>
	/// <param name="record">Запись действия для записи.</param>
	public void WriteRecord(ActionRecord record)
	{
		_recordsQueue.Enqueue(record);
	}

	/// <summary>
	/// Записывает несколько записей действий.
	/// </summary>
	/// <param name="records">Записи действий.</param>
	public void WriteRecords(IEnumerable<ActionRecord> records)
	{
		foreach (ActionRecord record in records)
		{
			WriteRecord(record);
		}
	}

	/// <summary>
	/// Читает ожидающие записи действий в транзакции.
	/// </summary>
	/// <param name="token">Токен отмены операции.</param>
	/// <returns>Транзакция с ожидающими записями действий.</returns>
	public async Task<TelemetryActionRecordsTransaction> ReadPendingRecordsTransaction(CancellationToken token)
	{
		await _semaphore.WaitAsync(cancellationToken: token);
		IDatabase database = await ReadDatabase();
		long length = await database.ListLengthAsync(ACTION_RECORDS_ARRAY_KEY);

		RedisValue[] records = await database.ListRangeAsync(ACTION_RECORDS_ARRAY_KEY, 0, length - 1);
		ActionRecord[] result = [.. records.Select(RedisValueToActionRecord)];

		ITransaction transaction = database.CreateTransaction();
		_semaphore.Release();
		return new(transaction, result);
	}

	/// <summary>
	/// Удаляет записи действий из Redis в транзакции.
	/// </summary>
	/// <param name="transaction">Транзакция с записями действий для удаления.</param>
	/// <returns>Завершение задачи удаления записей действий.</returns>
	public async Task RemoveRecords(TelemetryActionRecordsTransaction transaction)
	{
		await _semaphore.WaitAsync();
		(ITransaction tran, IReadOnlyList<ActionRecord> records) = transaction;
		List<Task> tasks = [];
		foreach (ActionRecord record in records)
		{
			tasks.Add(tran.ListRemoveAsync(ACTION_RECORDS_ARRAY_KEY, ActionRecordToRedisValue(record)));
		}

		tasks.Add(tran.ExecuteAsync());

		try
		{
			await Task.WhenAll(tasks).ConfigureAwait(false);
			Logger.Information("Successfully removed {Count} telemetry action records from Redis.", records.Count);
		}
		catch (Exception ex)
		{
			Logger.Fatal(ex, "Failed to remove telemetry action records from Redis.");
		}
		finally
		{
			_semaphore.Release();
		}
	}

	/// <summary>
	/// Асинхронно освобождает ресурсы.
	/// </summary>
	/// <returns>Выполненное действие освобождения ресурсов.</returns>
	public async ValueTask DisposeAsync()
	{
		await _timerCts.CancelAsync();
		await _runningPeriodicallyWritingTask;
		_timerCts.Dispose();
		_semaphore.Dispose();
	}

	/// <summary>
	/// Освобождает ресурсы.
	/// </summary>
	public void Dispose()
	{
		_timerCts.Cancel();
		_runningPeriodicallyWritingTask.Wait();
		_timerCts.Dispose();
		_semaphore.Dispose();
	}

	private static ActionRecord RedisValueToActionRecord(RedisValue value)
	{
		RedisStoredActionRecord stored = JsonSerializer.Deserialize<RedisStoredActionRecord>(value.ToString())!;
		return stored.ToActionRecord();
	}

	private static RedisValue ActionRecordToRedisValue(ActionRecord record)
	{
		RedisStoredActionRecord toStore = RedisStoredActionRecord.FromActionRecord(record);
		return JsonSerializer.Serialize(toStore);
	}

	private static ActionRecord[] MakeRecordsSnapshot(ConcurrentQueue<ActionRecord> recordsQueue)
	{
		List<ActionRecord> snapshot = [];
		for (int index = 0; recordsQueue.TryDequeue(out ActionRecord? record); index++)
		{
			if (record is null)
			{
				continue;
			}

			snapshot.Add(record);
		}

		return [.. snapshot];
	}

	private static async Task ProcessPendingRecords(IDatabase database, ActionRecord[] snapshot, ILogger logger)
	{
		if (snapshot.Length == 0)
		{
			return;
		}

		RedisValue[] recordsToPublish = new RedisValue[snapshot.Length];
		for (int index = 0; index < snapshot.Length; index++)
		{
			RedisStoredActionRecord toStore = RedisStoredActionRecord.FromActionRecord(snapshot[index]);
			recordsToPublish[index] = JsonSerializer.Serialize(toStore);
		}

		logger.Information("Writing {Count} telemetry action records to Redis.", snapshot.Length);

		try
		{
			await database.ListLeftPushAsync(ACTION_RECORDS_ARRAY_KEY, recordsToPublish, CommandFlags.FireAndForget);
			logger.Information("Successfully saved {Count} telemetry action records to Redis.", snapshot.Length);
		}
		catch (Exception ex)
		{
			logger.Fatal(ex, "Failed to write telemetry action records to Redis.");
		}
	}

	private async Task RunPeriodicProcessingAsync(CancellationToken ct)
	{
		using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));
		while (await timer.WaitForNextTickAsync(ct))
		{
			ActionRecord[] snapshot = MakeRecordsSnapshot(_recordsQueue);
			IDatabase database = await ReadDatabase();
			await ProcessPendingRecords(database, snapshot, Logger);
		}
	}

	private async Task<ConnectionMultiplexer> ReadMultiplexer()
	{
		return _multiplexer ??= await ConnectionMultiplexer.ConnectAsync(Options.RedisConnectionString);
	}

	private async Task<IDatabase> ReadDatabase()
	{
		ConnectionMultiplexer multiplexer = await ReadMultiplexer();
		return _database ??= multiplexer.GetDatabase();
	}

	private sealed class RedisStoredActionRecord
	{
		public required Guid Id { get; set; }
		public required Guid? InvokerId { get; set; }
		public required string? Error { get; set; }
		public required string Severity { get; set; }
		public required string Name { get; set; }
		public DateTime OccuredDateTime { get; set; }
		public string? PayloadJson { get; set; }

		public static RedisStoredActionRecord FromActionRecord(ActionRecord record)
		{
			return new RedisStoredActionRecord
			{
				Id = record.Id.Value,
				InvokerId = record.InvokerId?.Value,
				Error = record.Error?.Value,
				Severity = record.Severity.Value,
				Name = record.Name.Value,
				OccuredDateTime = record.OccuredDateTime.Value,
				PayloadJson = record.PayloadJson?.Value,
			};
		}

		public ActionRecord ToActionRecord()
		{
			return new ActionRecord
			{
				Id = ActionRecordId.Create(Id).Value,
				InvokerId = InvokerId.HasValue ? ActionRecordInvokerId.Create(InvokerId.Value).Value : null,
				Error = ActionRecordError.FromNullableString(Error),
				Severity = ActionRecordSeverity.Create(Severity).Value,
				Name = ActionRecordName.Create(Name),
				OccuredDateTime = ActionRecordOccuredDateTime.Create(OccuredDateTime).Value,
				PayloadJson = PayloadJson != null ? ActionRecordPayloadJson.Create(PayloadJson).Value : null,
			};
		}
	}
}
