using Npgsql;

namespace Scrapers.Module.Domain.JournalsContext.Persistance;

internal sealed class NpgSqlSavingScraperJournalOutputSource
    : IScraperJournalOutputSource<NpgSqlSavingScraperJournalOutputSource>
{
    private readonly NpgsqlCommand _command;

    private const string Sql = """
        INSERT INTO scrapers_module.scraper_journals
        (id, parser_name, parser_type, created_at, completed_at)
        VALUES
        (@id, @parser_name, @parser_type, @created_at, @completed_at)
        ON CONFLICT (id)
        DO UPDATE SET 
            parser_name = @parser_name,
            parser_type = @parser_type,
            created_at = @created_at,
            completed_at = @completed_at
        """;

    private readonly Guid _id;
    private readonly string _scraperName;
    private readonly string _scraperType;
    private readonly DateTime _createdAt;
    private readonly DateTime? _completedAt;

    public void Behave()
    {
        _command.CommandText = Sql;
        _command.Parameters.Add(new NpgsqlParameter<Guid>("@id", _id));
        _command.Parameters.Add(new NpgsqlParameter<string>("@parser_name", _scraperName));
        _command.Parameters.Add(new NpgsqlParameter<string>("@parser_type", _scraperType));
        _command.Parameters.Add(new NpgsqlParameter<DateTime>("@created_at", _createdAt));
        _command.Parameters.Add(
            _completedAt.HasValue
                ? new NpgsqlParameter<DateTime>("@completed_at", _completedAt.Value)
                : new NpgsqlParameter("@completed_at", DBNull.Value)
        );
    }

    public Task BehaveAsync()
    {
        Behave();
        return Task.CompletedTask;
    }

    public NpgSqlSavingScraperJournalOutputSource Accept(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    ) => new(_command, id, scraperName, scraperType, createdAt, completedAt);

    public NpgSqlSavingScraperJournalOutputSource(NpgsqlCommand command)
        : this(command, Guid.Empty, string.Empty, string.Empty, DateTime.MinValue, null) { }

    private NpgSqlSavingScraperJournalOutputSource(
        NpgsqlCommand command,
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    )
    {
        _command = command;
        _id = id;
        _scraperName = scraperName;
        _scraperType = scraperType;
        _createdAt = createdAt;
        _completedAt = completedAt;
    }
}
