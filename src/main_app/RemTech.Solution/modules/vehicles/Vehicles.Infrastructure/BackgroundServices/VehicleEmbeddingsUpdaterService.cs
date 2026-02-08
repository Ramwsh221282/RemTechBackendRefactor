using System.Data;
using Dapper;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Vehicles.Infrastructure.BackgroundServices;

/// <summary>
/// Сервис обновления эмбеддингов транспортных средств.
/// </summary>
public sealed class VehicleEmbeddingsUpdaterService(
	NpgSqlConnectionFactory npgSql,
	Serilog.ILogger logger,
	EmbeddingsProvider provider
) : BackgroundService
{
	private Serilog.ILogger Logger { get; } = logger.ForContext<VehicleEmbeddingsUpdaterService>();
	private EmbeddingsProvider Provider { get; } = provider;
	private NpgSqlConnectionFactory NpgSql { get; } = npgSql;

	/// <summary>
	/// Запускает фоновую задачу для обновления эмбеддингов транспортных средств.
	/// </summary>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
        while (!stoppingToken.IsCancellationRequested)
        {
        	try
        	{
        		await Execute(stoppingToken);
        	}
        	catch (Exception e)
        	{
        		Logger.Fatal(e, "Error updating vehicle embeddings.");
        	}
        	finally
        	{
        		await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        	}
        }
    }

	private static VehicleWithoutEmbedding MapFromReader(IDataReader reader)
	{
		return new() { Id = reader.GetValue<Guid>("id"), TextForEmbedding = reader.GetValue<string>("text") };
	}

	private async Task Execute(CancellationToken ct)
	{
		await using NpgSqlSession session = new(NpgSql);
		NpgSqlTransactionSource transactionSource = new(session);
		await using ITransactionScope transaction = await transactionSource.BeginTransaction(ct);
        
        IReadOnlyList<VehicleWithoutEmbedding> vehicles = await GetVehiclesWithoutEmbeddings(session, ct);
        if (vehicles.Count == 0)
		{
			return;
		}

		string[] texts = [.. vehicles.Select(v => v.TextForEmbedding)];
		IReadOnlyList<ReadOnlyMemory<float>> embeddings = Provider.GenerateBatch(texts);
		await UpdateVehicleEmbeddings(session, vehicles, embeddings, ct);
		Result commit = await transaction.Commit(ct);
		if (commit.IsFailure)
		{
			Logger.Fatal(commit.Error, "Error committing transaction.");
		}
	}

	private async Task UpdateVehicleEmbeddings(
		NpgSqlSession session,
		IReadOnlyList<VehicleWithoutEmbedding> vehicles,
		IReadOnlyList<ReadOnlyMemory<float>> embeddings,
		CancellationToken ct
	)
	{
		List<string> updateClauses = [];
		DynamicParameters parameters = new();
		Guid[] ids = new Guid[vehicles.Count];
		for (int i = 0; i < vehicles.Count; i++)
		{
			ids[i] = vehicles[i].Id;
			string idParam = $"@id_{i}";
			string embeddingParam = $"@embedding_{i}";
			updateClauses.Add($"WHEN v.id = {idParam} THEN {embeddingParam}");
			parameters.Add(idParam, vehicles[i].Id, DbType.Guid);
			parameters.Add(embeddingParam, new Vector(embeddings[i]));
		}

		parameters.Add("@ids", ids);
		string updateClause = string.Join(" ", updateClauses);
		string updateSql = $"""
			UPDATE vehicles_module.vehicles v
			SET embedding = CASE {updateClause} ELSE v.embedding END
			WHERE v.id = ANY(@ids)
			""";
		CommandDefinition command = new(updateSql, parameters, transaction: session.Transaction);
		NpgsqlConnection connection = await session.GetConnection(ct);
		int updated = await connection.ExecuteAsync(command);
		Logger.Information("Updated {Count} vehicles", updated);
	}

	private async Task<IReadOnlyList<VehicleWithoutEmbedding>> GetVehiclesWithoutEmbeddings(
		NpgSqlSession session,
		CancellationToken ct
	)
	{
		const string sql = """
			SELECT
			    v.id as Id,
			    (c.name || ' ' || b.name || ' ' || m.name || ' ' || r.name || ' ' || r.kind || ' ' || 'Характеристики: ' || characteristics.ctx_name) as TextForEmbedding
			FROM vehicles_module.vehicles v
			         JOIN vehicles_module.categories c ON v.category_id = c.id
			         JOIN vehicles_module.brands b ON v.brand_id = b.id
			         JOIN vehicles_module.models m ON v.model_id = m.id
			         JOIN vehicles_module.regions r ON v.region_id = r.id
			         JOIN LATERAL (
			             SELECT string_agg((ctx.name || ': ' || vc.value), ' ') as ctx_name FROM vehicles_module.vehicle_characteristics vc
			             JOIN vehicles_module.characteristics ctx ON vc.characteristic_id = ctx.id
			             WHERE vc.vehicle_id = v.id
			
			         ) AS characteristics ON true
			WHERE v.embedding IS NULL
			LIMIT 50;
			""";
		CommandDefinition command = new(sql, transaction: session.Transaction, cancellationToken: ct);
        NpgsqlConnection connection = await session.GetConnection(ct);
        IEnumerable<VehicleWithoutEmbedding> vehicles = await connection.QueryAsync<VehicleWithoutEmbedding>(command);

        List<VehicleWithoutEmbedding> list = vehicles.ToList();
		Logger.Information("Found {Count} vehicles without embedding", list.Count);
        return list;
    }

	private sealed class VehicleWithoutEmbedding
	{
		public required Guid Id { get; set; }
		public required string TextForEmbedding { get; set; }
	}
}
