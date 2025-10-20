using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Domain.JournalsContext.Logging;
using Scrapers.Module.Domain.JournalsContext.Persistance;

namespace Scrapers.Module.Domain.JournalsContext.Features.CreateScraperJournal;

internal sealed class CreateScraperJournalHandler(
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger,
    ActiveScraperJournalsCache cache
) : ICommandHandler<CreateScraperJournalCommand>
{
    public async Task Handle(CreateScraperJournalCommand command, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        ScraperJournal journal = ScraperJournal.Create(command.ScraperName, command.ScraperType);
        NpgSqlSavingScraperJournalOutputSource sqlOutput = new(sqlCommand);
        sqlOutput = journal.PrintTo(sqlOutput);
        await sqlOutput.BehaveAsync();
        await sqlCommand.ExecuteNonQueryAsync(ct);
        LoggingScraperJournalOutputSource log = new(logger);
        log = journal.PrintTo(log);
        await log.BehaveAsync();
        await cache.SaveIdentifier(journal);
    }
}
