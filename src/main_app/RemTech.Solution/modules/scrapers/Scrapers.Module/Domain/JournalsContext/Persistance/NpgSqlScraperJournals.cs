using System.Data.Common;
using System.Text;
using Npgsql;
using Scrapers.Module.Domain.JournalsContext.Exceptions;

namespace Scrapers.Module.Domain.JournalsContext.Persistance;

internal sealed class NpgSqlScraperJournals(NpgsqlConnection connection) : IScraperJournals
{
    private const string SqlGetJournalById = """
        SELECT 
        j.id as journal_id, 
        j.parser_name as journal_parser_name,
        j.parser_type as journal_parser_type,
        j.created_at as journal_created_at,
        j.completed_at as journal_completed_at,
        r.id as record_id,
        r.action as record_action,
        r.text as record_text,
        r.created_at as record_created_at
        FROM scrapers_module.scraper_journals j
        LEFT JOIN scrapers_module.journal_records r ON j.id = r.journal_id
        WHERE j.id = @id;
        """;

    public async Task<ScraperJournal> ById(Guid id, CancellationToken ct = default)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = SqlGetJournalById;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!reader.HasRows)
            throw new ScraperJournalByIdNotFoundException();
        Dictionary<Guid, ScraperJournal> journalData = [];
        while (await reader.ReadAsync(ct))
        {
            Guid journalId = reader.GetGuid(0);
            if (journalData.Count == 0)
            {
                string scraperName = reader.GetString(1);
                string scraperType = reader.GetString(2);
                DateTime createdAt = reader.GetDateTime(3);
                DateTime? completedAt = await reader.IsDBNullAsync(4, ct)
                    ? null
                    : reader.GetDateTime(4);
                ScraperJournal journal = ScraperJournal.Create(
                    journalId,
                    scraperName,
                    scraperType,
                    createdAt,
                    completedAt,
                    []
                );
                journalData.Add(journalId, journal);
            }

            if (!await reader.IsDBNullAsync(5, ct))
            {
                Guid recordId = reader.GetGuid(5);
                string action = reader.GetString(6);
                string text = reader.GetString(7);
                DateTime recordCreatedAt = reader.GetDateTime(8);
                if (journalData.ContainsKey(journalId))
                    journalData[journalId].WithRecord(recordId, action, text, recordCreatedAt);
            }
        }
        return journalData.Count == 0
            ? throw new ScraperJournalByIdNotFoundException()
            : journalData.First().Value;
    }

    private const string RemoveSql = "DELETE FROM scrapers_module.scraper_journals where id = @id";

    public async Task RemoveById(Guid id, CancellationToken ct = default)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = RemoveSql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", id));
        int affected = await command.ExecuteNonQueryAsync(ct);
        if (affected == 0)
            throw new ScraperJournalByIdNotFoundException();
    }

    private const string GetPagedSql = """ 
        SELECT id, parser_name, parser_type, created_at, completed_at   
        FROM scrapers_module.scraper_journals
        """;

    private const int MaxPageSize = 20;

    public async Task<IEnumerable<ScraperJournal>> GetPaged(
        string name,
        string type,
        int page,
        DateTime? from,
        DateTime? to,
        CancellationToken ct = default
    )
    {
        if (page < 1 || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
            return [];
        await using NpgsqlCommand command = connection.CreateCommand();
        StringBuilder sb = new StringBuilder(GetPagedSql);
        sb.AppendLine(" WHERE 1=1 AND parser_name = @name AND parser_type = @type ");
        if (from.HasValue)
        {
            sb.AppendLine(" AND created_at >= @from ");
            command.Parameters.Add(new NpgsqlParameter<DateTime>("@from", from.Value));
        }
        if (to.HasValue)
        {
            sb.AppendLine(" AND completed_at <= @to ");
            command.Parameters.Add(new NpgsqlParameter<DateTime>("@to", to.Value));
        }
        sb.AppendLine(" ORDER BY completed_at DESC NULLS LAST ");
        sb.AppendLine(" LIMIT @limit OFFSET @offset ");
        int offset = (page - 1) * MaxPageSize;
        command.Parameters.Add(new NpgsqlParameter<int>("@limit", MaxPageSize));
        command.Parameters.Add(new NpgsqlParameter<int>("@offset", offset));
        command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", type));
        command.CommandText = sb.ToString();
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        List<ScraperJournal> journals = [];
        while (await reader.ReadAsync(ct))
        {
            Guid id = reader.GetGuid(0);
            string parserName = reader.GetString(1);
            string parserType = reader.GetString(2);
            DateTime createdAt = reader.GetDateTime(3);
            DateTime? completedAt = await reader.IsDBNullAsync(4, ct)
                ? null
                : reader.GetDateTime(4);
            journals.Add(
                ScraperJournal.Create(id, parserName, parserType, createdAt, completedAt, [])
            );
        }
        return journals;
    }

    private const string CountSql = """
        SELECT COUNT(id) FROM scrapers_module.scraper_journals
        WHERE parser_name = @name AND parser_type = @type
        """;

    public async Task<long> GetCount(string name, string type, CancellationToken ct = default)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = CountSql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", name));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", type));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);
        long count = reader.GetInt64(0);
        return count;
    }
}
