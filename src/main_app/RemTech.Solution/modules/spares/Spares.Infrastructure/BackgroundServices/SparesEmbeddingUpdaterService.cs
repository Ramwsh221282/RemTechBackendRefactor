using System.Data;
using Dapper;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Pgvector;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.NN;

namespace Spares.Infrastructure.BackgroundServices;

public sealed class SparesEmbeddingUpdaterService(
    Serilog.ILogger logger, 
    EmbeddingsProvider embeddings, 
    NpgSqlConnectionFactory npgSql)
    : BackgroundService
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<SparesEmbeddingUpdaterService>();
    private EmbeddingsProvider Embeddings { get; } = embeddings;
    private NpgSqlConnectionFactory NpgSql { get; } = npgSql;
    
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Execute(stoppingToken);
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Error updating spare embeddings.");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private async Task Execute(CancellationToken ct)
    {
        await using NpgSqlSession session = new(NpgSql);
        NpgSqlTransactionSource transactionSource = new(session);
        await using ITransactionScope transaction = await transactionSource.BeginTransaction(ct);
        SpareWithoutEmbedding[] spares = await GetSparesWithoutEmbeddings(session, ct);
        if (spares.Length == 0) return;
        
        string[] texts = spares.Select(s => s.TextForEmbedding).ToArray();
        IReadOnlyList<ReadOnlyMemory<float>> embeddings = Embeddings.GenerateBatch(texts);
        await UpdateSpareEmbeddings(session, spares, embeddings, ct);
        Result commit = await transaction.Commit(ct);
        if (commit.IsFailure) Logger.Fatal(commit.Error, "Error committing transaction.");
    }

    private async Task UpdateSpareEmbeddings(
        NpgSqlSession session,
        SpareWithoutEmbedding[] spares, 
        IReadOnlyList<ReadOnlyMemory<float>> embeddings,
        CancellationToken ct)
    {
        List<string> updateClauses = [];
        DynamicParameters parameters = new();
        Guid[] ids = new Guid[spares.Length];
        for (int i = 0; i < spares.Length; i++)
        {
            ids[i] = spares[i].Id;
            string idParam = $"@id_{i}";
            string embeddingParam = $"@embedding_{i}";
            updateClauses.Add($"WHEN s.id = {idParam} THEN {embeddingParam}");
            parameters.Add(idParam, spares[i].Id, DbType.Guid);
            parameters.Add(embeddingParam, new Vector(embeddings[i]));
        }
        
        parameters.Add("@ids", ids);
        string updateClause = string.Join(" ", updateClauses);
        string updateSql = $"""
                            UPDATE spares_module.spares s 
                            SET embedding = CASE {updateClause} ELSE s.embedding END 
                            WHERE s.id = ANY(@ids)
                            """;
        CommandDefinition command = new(updateSql, parameters, transaction: session.Transaction);
        NpgsqlConnection connection = await session.GetConnection(ct);
        int updated = await connection.ExecuteAsync(command);
        Logger.Information("Updated {Count} spares", updated);
    }
    
    private async Task<SpareWithoutEmbedding[]> GetSparesWithoutEmbeddings(NpgSqlSession session, CancellationToken ct)
    {
        const string sql = """
                           SELECT
                               s.id as id,
                               (s.text || ' ' || s.type || ' ' || s.oem || ' ' || r.name || ' ' || r.kind) as text
                           FROM spares_module.spares s
                           JOIN vehicles_module.regions r ON s.region_id = r.id
                           WHERE s.embedding IS NULL
                           LIMIT 50
                           FOR UPDATE OF s;
                           """;
        CommandDefinition command = new(sql, transaction: session.Transaction, cancellationToken: ct);
        SpareWithoutEmbedding[] spares = await session.QueryMultipleUsingReader(command, MapFromReader);
        Logger.Information("Found {Count} spares without embedding", spares.Length);
        return spares;
    }

    private static SpareWithoutEmbedding MapFromReader(IDataReader reader) => new()
    {
        Id = reader.GetValue<Guid>("id"),
        TextForEmbedding = reader.GetValue<string>("text")
    };
    
    private sealed class SpareWithoutEmbedding
    {
        public required Guid Id { get; set; }
        public required string TextForEmbedding { get; set; }
    }
}