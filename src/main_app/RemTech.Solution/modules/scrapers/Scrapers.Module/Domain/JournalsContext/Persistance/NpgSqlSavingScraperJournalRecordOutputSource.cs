using Npgsql;
using Pgvector;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Scrapers.Module.Domain.JournalsContext.Persistance;

internal sealed class NpgSqlSavingScraperJournalRecordOutputSource
    : IScraperJournalRecordOutputSource<NpgSqlSavingScraperJournalRecordOutputSource>
{
    private const string Sql = """
        INSERT INTO scrapers_module.journal_records
        (id, journal_id, action, text, created_at, embedding)
        VALUES
        (@id, @journal_id, @action, @text, @created_at, @embedding);
        """;
    private readonly NpgsqlCommand _command;
    private readonly IEmbeddingGenerator _generator;
    private readonly Guid _id;
    private readonly Guid _journalId;
    private readonly string _action;
    private readonly string _text;
    private readonly DateTime _createdAt;

    public NpgSqlSavingScraperJournalRecordOutputSource Accept(
        Guid id,
        Guid journalId,
        string action,
        string text,
        DateTime createdAt
    ) => new(_command, _generator, id, journalId, action, text, createdAt);

    public void Behave()
    {
        _command.CommandText = Sql;
        _command.Parameters.Add(new NpgsqlParameter<Guid>("@id", _id));
        _command.Parameters.Add(new NpgsqlParameter<Guid>("@journal_id", _journalId));
        _command.Parameters.Add(new NpgsqlParameter<string>("@action", _action));
        _command.Parameters.Add(new NpgsqlParameter<string>("@text", _text));
        _command.Parameters.Add(new NpgsqlParameter<DateTime>("@created_at", _createdAt));
        _command.Parameters.AddWithValue(
            "@embedding",
            new Vector(_generator.Generate($"Действие: {_action}, {_text}"))
        );
    }

    public Task BehaveAsync()
    {
        Behave();
        return Task.CompletedTask;
    }

    public NpgSqlSavingScraperJournalRecordOutputSource(
        NpgsqlCommand command,
        IEmbeddingGenerator generator
    )
        : this(
            command,
            generator,
            Guid.Empty,
            Guid.Empty,
            string.Empty,
            string.Empty,
            DateTime.MinValue
        ) { }

    private NpgSqlSavingScraperJournalRecordOutputSource(
        NpgsqlCommand command,
        IEmbeddingGenerator generator,
        Guid id,
        Guid journalId,
        string action,
        string text,
        DateTime createdAt
    )
    {
        _command = command;
        _generator = generator;
        _id = id;
        _journalId = journalId;
        _action = action;
        _text = text;
        _createdAt = createdAt;
    }
}
