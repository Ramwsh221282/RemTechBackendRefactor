using System.Text;
using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;
using Telemetry.Core.ActionRecords;
using Telemetry.Core.ActionRecords.ValueObjects;

namespace Telemetry.Infrastructure;

/// <summary>
/// Фоновый процесс для сохранения записей действий телеметрии.
/// </summary>
/// <param name="logger">Логгер для записи информации о процессе.</param>
/// <param name="storage">Читатель записей действий из Redis.</param>
/// <param name="embeddings">Провайдер эмбеддингов.</param>
/// /// <param name="npgSql">Провайдер эмбеддингов.</param>
public sealed class ActionRecordsPersistingBackgroundProcess(
	Serilog.ILogger logger,
	RedisTelemetryActionsStorage storage,
	EmbeddingsProvider embeddings,
	NpgSqlConnectionFactory npgSql
) : BackgroundService
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<ActionRecordsPersistingBackgroundProcess>();
	private RedisTelemetryActionsStorage Storage { get; } = storage;
	private EmbeddingsProvider Embeddings { get; } = embeddings;
	private NpgSqlConnectionFactory NpgSql { get; } = npgSql;

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
			await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
		}
	}

	private static string GetTextForEmbedding(ActionRecord record)
	{
		StringBuilder stringBuilder = new StringBuilder()
			.Append("Действие пользователя")
			.Append("направлено на следующий ресурс: ")
			.Append(record.Name.Value)
			.Append($". Было выполнено {GetSeverityText(record.Severity)} ")
			.Append($"{ErrorText(record)}");
		return stringBuilder.ToString();
	}

	private static string ErrorText(ActionRecord record)
	{
		return record.Error is null ? string.Empty : $" Ошибка: {record.Error.Value}.";
	}

	private static JsonElement? Payload(ActionRecord record)
	{
		if (record.PayloadJson is null)
		{
			return null;
		}

		if (string.IsNullOrWhiteSpace(record.PayloadJson.Value))
		{
			return null;
		}

		using JsonDocument doc = JsonDocument.Parse(record.PayloadJson.Value);
		return doc.RootElement.Clone();
	}

	private static string GetSeverityText(ActionRecordSeverity severity)
	{
		return severity.Equals(ActionRecordSeverity.ERROR) ? "с ошибкой."
			: severity.Equals(ActionRecordSeverity.SUCCESS) ? "с успехом."
			: severity.Equals(ActionRecordSeverity.INFO) ? "с успехом."
			: severity.Equals(ActionRecordSeverity.WARNING) ? "с предупреждением."
			: "неизвестно.";
	}

	private async Task ProcessActionRecordsAsync(CancellationToken token)
	{
		Logger.Information("Background service processing action records.");
		try
		{
			TelemetryActionRecordsTransaction transaction = await Storage.ReadPendingRecordsTransaction(token);
			Result cacheResult = await ProcessOnCacheLevelSide(transaction);
			if (cacheResult.IsFailure)
			{
				string error = cacheResult.Error.Message;
				Logger.Fatal("Failed to process action records on cache level side. Error: {Error}", error);
				return;
			}

			Result dbResult = await ProcessOnDatabaseLevelSide(transaction.Records);
			if (dbResult.IsFailure)
			{
				string error = dbResult.Error.Message;
				Logger.Fatal("Failed to process action records on database level side. Error: {Error}", error);
				Logger.Information("Re-adding {Count} action records back to Redis.", transaction.Records.Count);
				Storage.WriteRecords(transaction.Records);
				return;
			}
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Error processing action records.");
			return;
		}
	}

	private async Task<Result> ProcessOnCacheLevelSide(TelemetryActionRecordsTransaction transaction)
	{
		if (transaction.Records.Count == 0)
		{
			Logger.Information("No action records to process on cache level side.");
			return Result.Success();
		}

		try
		{
			int processingRecordsCount = transaction.Records.Count;
			await Storage.RemoveRecords(transaction);
			Logger.Information("Removed {Count} action records.", processingRecordsCount);
			return Result.Success();
		}
		catch (Exception ex)
		{
			Logger.Error(ex, "Error processing action records on cache level side.");
			return Result.Failure(Error.Application(string.Empty));
		}
	}

	private async Task<Result> ProcessOnDatabaseLevelSide(IReadOnlyList<ActionRecord> records)
	{
		if (records.Count == 0)
		{
			Logger.Information("No action records to persist.");
			return Result.Success();
		}

		IReadOnlyList<string> texts = [.. records.Select(GetTextForEmbedding)];
		IReadOnlyList<ReadOnlyMemory<float>> embeddings = Embeddings.GenerateBatch(texts);
		var parameters = records.Select(
			(record, index) =>
				new
				{
					id = record.Id.Value,
					invoker_id = !record.InvokerId.HasValue ? (object)null : record.InvokerId.Value.Value,
					error = record.Error is null ? (object)null : record.Error.Value,
					severity = record.Severity.Value,
					name = record.Name.Value,
					created_at = record.OccuredDateTime.Value,
					payload = Payload(record) ?? (object)null,
					embedding = new Vector(embeddings[index]),
				}
		);

		const string sql = """
			INSERT INTO telemetry_module.action_records
			(id, invoker_id, error, severity, name, created_at, payload, embedding)
			VALUES
			(@id, @invoker_id, @error, @severity, @name, @created_at, @payload, @embedding);
			""";

		NpgSqlSession session = new(NpgSql);
		NpgsqlConnection connection = await session.GetConnection(CancellationToken.None);
		NpgSqlTransactionSource transactionSource = new(session);
		ITransactionScope transaction = await transactionSource.BeginTransaction();

		try
		{
			await connection.ExecuteAsync(sql, parameters, transaction: session.Transaction);

			Result commit = await transaction.Commit();
			if (commit.IsFailure)
			{
				Logger.Information("Failed to commit transaction for {Count} telemetry action records.", records.Count);
				return commit;
			}

			Logger.Information("Successfully persisted {Count} telemetry action records to database.", records.Count);
			return commit;
		}
		catch (Exception ex)
		{
			Logger.Fatal(ex, "Failed to persist telemetry action records to database.");
			return Result.Failure(Error.Application(string.Empty));
		}
		finally
		{
			await session.DisposeAsync();
		}
	}
}
