using System.Data.Common;
using System.Text;
using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Scrapers.Module.Domain.JournalsContext.Persistance;

internal sealed class NpgSqlScraperJournalRecords(
    NpgsqlConnection connection,
    IEmbeddingGenerator generator
) : IScraperJournalRecords
{
    private const int MaxPageSize = 50;

    private const string Sql = """
        SELECT id, journal_id, action, text, created_at
        FROM scrapers_module.journal_records
        WHERE journal_id = @journal_id
        """;

    public async Task<IEnumerable<ScraperJournalRecord>> GetPaged(
        Guid journalId,
        int page,
        string? text = null,
        CancellationToken ct = default
    )
    {
        if (page < 1 || journalId == Guid.Empty)
            return [];
        StringBuilder sb = new StringBuilder(Sql);
        await using NpgsqlCommand command = connection.CreateCommand();
        if (!string.IsNullOrEmpty(text))
        {
            sb.AppendLine(" ORDER BY embedding <=> @embedding ASC, created_at DESC ");
            command.Parameters.AddWithValue("@embedding", new Vector(generator.Generate(text)));
        }
        else
        {
            sb.AppendLine(" ORDER BY created_at DESC ");
        }
        int offset = (page - 1) * MaxPageSize;
        sb.AppendLine(" LIMIT @limit OFFSET @offset ");
        command.Parameters.Add(new NpgsqlParameter<Guid>("@journal_id", journalId));
        command.Parameters.Add(new NpgsqlParameter<int>("@offset", offset));
        command.Parameters.Add(new NpgsqlParameter<int>("@limit", MaxPageSize));
        command.CommandText = sb.ToString();
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        List<ScraperJournalRecord> journals = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(0);
            Guid recordJournalId = reader.GetGuid(1);
            string action = reader.GetString(2);
            string recordText = reader.GetString(3);
            DateTime createdAt = reader.GetDateTime(4);
            ScraperJournalRecord record = new(id, recordJournalId, action, recordText, createdAt);
            journals.Add(record);
        }
        return journals;
    }

    private const string CountSql = """
        SELECT COUNT(id) FROM scrapers_module.journal_records
        WHERE journal_id = @journal_id
        """;

    public async Task<long> GetCount(Guid journalId, CancellationToken ct = default)
    {
        if (journalId == Guid.Empty)
            return 0;
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = CountSql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@journal_id", journalId));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);
        long amount = reader.GetInt64(0);
        return amount;
    }
}
