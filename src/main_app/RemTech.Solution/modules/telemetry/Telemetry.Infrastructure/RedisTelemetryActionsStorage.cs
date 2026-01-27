using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Infrastructure.Redis;
using Serilog;
using StackExchange.Redis;
using Telemetry.Core.ActionRecords;

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
	private bool IsProcessing { get; set; }
	private IDatabase? _database { get; set; }
	private CachingOptions Options { get; }

	/// <summary>
	/// Записывает запись действия.
	/// </summary>
	/// <param name="record">Запись действия для записи.</param>
	public void WriteRecord(ActionRecord record) => _recordsQueue.Enqueue(record);

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
		ITransaction transaction = database.CreateTransaction();
		ActionRecord[] result = RedisValuesToActionRecords(records);
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
		foreach (ActionRecord record in records)
		{
			await tran.ListRemoveAsync(ACTION_RECORDS_ARRAY_KEY, ActionRecordToRedisValue(record), 1);
		}

		try
		{
			await tran.ExecuteAsync();
			Logger.Information("Successfully removed {Count} telemetry action records from Redis.", records.Count);
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Failed to remove telemetry action records from Redis.");
			foreach (ActionRecord record in records)
			{
				WriteRecord(record);
			}
		}

		_semaphore.Release();
	}

	/// <summary>
	/// Асинхронно освобождает ресурсы.
	/// </summary>
	/// <returns>Выполненное действие освобождения ресурсов.</returns>
	public async ValueTask DisposeAsync()
	{
		await _timerCts.CancelAsync();
		await _runningPeriodicallyWritingTask;
	}

	/// <summary>
	/// Освобождает ресурсы.
	/// </summary>
	public void Dispose()
	{
		_timerCts.Cancel();
		_runningPeriodicallyWritingTask.Wait();
	}

	private static ActionRecord[] RedisValuesToActionRecords(RedisValue[] values) =>
		Array.ConvertAll(values, RedisValueToActionRecord);

	private static RedisValue ActionRecordToRedisValue(ActionRecord record) => JsonSerializer.Serialize(record);

	private static RedisValue[] ActionRecordsToRedisValues(IEnumerable<ActionRecord> records) =>
		[.. records.Select(ActionRecordToRedisValue)];

	private static ActionRecord RedisValueToActionRecord(RedisValue value) =>
		JsonSerializer.Deserialize<ActionRecord>(value.ToString())!;

	private static ActionRecord[] MakeRecordsSnapshot(ConcurrentQueue<ActionRecord> recordsQueue)
	{
		List<ActionRecord> snapshot = [];
		for (int index = 0; recordsQueue.TryDequeue(out ActionRecord record); index++)
		{
			snapshot.Add(record);
		}

		return [.. snapshot];
	}

	private static async Task ProcessPendingRecords(IDatabase database, ActionRecord[] snapshot, ILogger logger)
	{
		if (snapshot.Length == 0)
			return;

		RedisValue[] recordsToPublish = new RedisValue[snapshot.Length];
		for (int index = 0; index < snapshot.Length; index++)
		{
			recordsToPublish[index] = JsonSerializer.Serialize(snapshot[index]);
		}

		logger.Information("Writing {Count} telemetry action records to Redis.", snapshot.Length);

		try
		{
			await database.ListLeftPushAsync(ACTION_RECORDS_ARRAY_KEY, recordsToPublish, CommandFlags.FireAndForget);
			logger.Information("Successfully wrote {Count} telemetry action records to Redis.", snapshot.Length);
		}
		catch (Exception ex)
		{
			logger.Fatal(ex, "Failed to write telemetry action records to Redis.");
		}
	}

	private async Task RunPeriodicProcessingAsync(CancellationToken ct)
	{
		PeriodicTimer timer = new(TimeSpan.FromSeconds(60));
		while (await timer.WaitForNextTickAsync(ct))
		{
			if (IsProcessing)
				continue;

			IsProcessing = true;
			ActionRecord[] snapshot = MakeRecordsSnapshot(_recordsQueue);
			IDatabase database = await ReadDatabase();
			await ProcessPendingRecords(database, snapshot, Logger);
			IsProcessing = false;
		}
	}

	private async Task<ConnectionMultiplexer> ReadMultiplexer() =>
		_multiplexer ??= await ConnectionMultiplexer.ConnectAsync(Options.RedisConnectionString);

	private async Task<IDatabase> ReadDatabase()
	{
		ConnectionMultiplexer multiplexer = await ReadMultiplexer();
		return _database ??= multiplexer.GetDatabase();
	}
}
