using Npgsql;

namespace Scrapers.Module.Domain.JournalsContext.Persistance;

internal sealed class NpgSqlRemovingScraperJournalOutput
    : IScraperJournalOutputSource<NpgSqlRemovingScraperJournalOutput>
{
    private const string Sql = "DELETE FROM scrapers_module.scraper_journals WHERE id = @id;";
    private readonly NpgsqlCommand _command;
    private readonly Guid _id;

    public void Behave()
    {
        _command.Parameters.Add(new NpgsqlParameter<Guid>("@id", _id));
        _command.CommandText = Sql;
    }

    public Task BehaveAsync()
    {
        Behave();
        return Task.CompletedTask;
    }

    public NpgSqlRemovingScraperJournalOutput Accept(
        Guid id,
        string scraperName,
        string scraperType,
        DateTime createdAt,
        DateTime? completedAt
    ) => new(_command, id);

    public NpgSqlRemovingScraperJournalOutput(NpgsqlCommand command)
        : this(command, Guid.Empty) { }

    private NpgSqlRemovingScraperJournalOutput(NpgsqlCommand command, Guid id)
    {
        _command = command;
        _id = id;
    }
}
