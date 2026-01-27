using Microsoft.Extensions.Hosting;

namespace Telemetry.Infrastructure;

/// <summary>
/// Фоновый процесс для сохранения записей действий телеметрии.
/// </summary>
/// <param name="logger">Логгер для записи информации о процессе.</param>
/// <param name="storage">Читатель записей действий из Redis.</param>
public sealed class ActionRecordsPersistingBackgroundProcess(
	Serilog.ILogger logger,
	RedisTelemetryActionsStorage storage
) : BackgroundService
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<ActionRecordsPersistingBackgroundProcess>();
	private RedisTelemetryActionsStorage Storage { get; } = storage;

	/// <summary>
	/// Основной метод фонового процесса.
	/// </summary>
	/// <param name="stoppingToken">Cancellation token.</param>
	/// <returns>Completed task.</returns>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await ProcessActionRecordsAsync(stoppingToken);
			await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
		}
	}

	private async Task ProcessActionRecordsAsync(CancellationToken token)
	{
		Logger.Information("Background service processing action records.");
		try
		{
			TelemetryActionRecordsTransaction transaction = await Storage.ReadPendingRecordsTransaction(token);
			int processingRecordsCount = transaction.Records.Count;
			await Storage.RemoveRecords(transaction);
			// TODO: Добавить логику сохранения записей действий в базу данных или другое хранилище.
			Logger.Information("Processed {Count} action records.", processingRecordsCount);
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Error processing action records.");
			return;
		}
	}
}
