using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RemTech.SharedKernel.Infrastructure.Redis;
using Serilog;
using StackExchange.Redis;
using Telemetry.Core.ActionRecords;

namespace Telemetry.Infrastructure;

// TODO: реализовать отправку логов сюда
// TODO: реализовать чтение логово отсюда, чтобы записывать в бд.

/// <summary>
/// Запись действий телеметрии в Redis.
/// </summary>
public sealed class RedisTelemetryActionsWriter : IDisposable, IAsyncDisposable
{
	private readonly ConcurrentQueue<ActionRecord> _recordsQueue = [];
	private ConnectionMultiplexer? _multiplexer { get; set; }
	private readonly CancellationTokenSource _timerCts = new();
	private readonly Task _runningPeriodicallyWritingTask;

	/// <summary>
	/// Создает новый экземпляр <see cref="RedisTelemetryActionsWriter"/>.
	/// </summary>
	/// <param name="options">Опции кэширования.</param>
	/// <param name="logger">Логгер для записи логов.</param>
	public RedisTelemetryActionsWriter(IOptions<CachingOptions> options, ILogger logger)
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
			await database.ListLeftPushAsync("Telemetry:ActionRecords", recordsToPublish, CommandFlags.FireAndForget);
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
