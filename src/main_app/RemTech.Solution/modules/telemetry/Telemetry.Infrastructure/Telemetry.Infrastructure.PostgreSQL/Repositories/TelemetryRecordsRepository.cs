using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pgvector;
using RemTech.Result.Pattern;
using Shared.Infrastructure.Module.Postgres.Embeddings;
using Telemetry.Domain.TelemetryContext;
using Telemetry.Domain.TelemetryContext.Contracts;
using Telemetry.Domain.TelemetryContext.ValueObjects;

namespace Telemetry.Infrastructure.PostgreSQL.Repositories;

public sealed class TelemetryRecordsRepository : ITelemetryRecordsRepository
{
    private readonly IEmbeddingGenerator _generator;
    private readonly TelemetryServiceDbContext _dbContext;

    public TelemetryRecordsRepository(
        IEmbeddingGenerator generator,
        TelemetryServiceDbContext dbContext
    )
    {
        _generator = generator;
        _dbContext = dbContext;
    }

    public async Task Add(TelemetryRecord record, CancellationToken ct = default)
    {
        _dbContext.Add(record);
        await _dbContext.SaveChangesAsync(ct);
        await UpdateEmbeddingForRecord(record, ct);
    }

    public async Task<Result<TelemetryRecord>> GetById(Guid id, CancellationToken ct = default)
    {
        Result<TelemetryRecordId> recordId = TelemetryRecordId.Create(id);
        return recordId.IsFailure ? recordId.Error : await GetById(recordId, ct);
    }

    public async Task<Result<TelemetryRecord>> GetById(
        TelemetryRecordId recordId,
        CancellationToken ct = default
    )
    {
        TelemetryRecord? record = await _dbContext
            .Records.AsNoTracking()
            .FirstOrDefaultAsync(r => r.RecordId == recordId, ct);
        return record
            ?? Result<TelemetryRecord>.Failure(
                new Error($"Запись действия не найдена с ID: {recordId.Value}", ErrorCodes.NotFound)
            );
    }

    public async Task<IEnumerable<TelemetryRecord>> GetByName(
        string name,
        CancellationToken ct = default
    )
    {
        Result<TelemetryActionName> recordName = TelemetryActionName.Create(name);
        return recordName.IsFailure ? [] : await GetByName(recordName, ct);
    }

    public async Task<IEnumerable<TelemetryRecord>> GetByName(
        TelemetryActionName name,
        CancellationToken ct = default
    )
    {
        // запрос с учетом jsonb колонки details, для получения записей с таким названием действия.
        IEnumerable<TelemetryRecord> records = await _dbContext
            .Records.FromSqlInterpolated(
                $@"
            SELECT * FROM telemetry_module.records 
                     WHERE details -> 'Name' ->> 'Value' = {name.Value} "
            )
            .AsNoTracking()
            .ToListAsync(ct);
        return records;
    }

    /// <summary>
    /// метод для обновления вектора у записи.
    /// </summary>
    private async Task UpdateEmbeddingForRecord(
        TelemetryRecord record,
        CancellationToken ct = default
    )
    {
        const string sql =
            "UPDATE telemetry_module.records SET embedding = @embedding WHERE id = @id";
        await using var connection = (NpgsqlConnection)_dbContext.Database.GetDbConnection();
        await connection.OpenAsync(ct);
        await using var command = new NpgsqlCommand();

        string comments = string.Join(", ", record.Details.Comments.Select(c => c.Value));
        string text =
            $"{record.Details.Name} {record.Status.Value} {comments} записано: {record.OccuredAt}";
        Vector vector = new Vector(_generator.Generate(text));
        command.Connection = connection;
        command.CommandText = sql;
        command.Parameters.AddWithValue("@embedding", vector);
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", record.RecordId.Value));
        await command.ExecuteNonQueryAsync(ct);
    }
}
