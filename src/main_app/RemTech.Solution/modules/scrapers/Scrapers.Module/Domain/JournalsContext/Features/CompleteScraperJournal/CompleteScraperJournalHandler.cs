using System.Globalization;
using Npgsql;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Domain.JournalsContext.Logging;
using Scrapers.Module.Domain.JournalsContext.Persistance;
using Shared.Infrastructure.Module.Cqrs;

namespace Scrapers.Module.Domain.JournalsContext.Features.CompleteScraperJournal;

internal sealed class CompleteScraperJournalHandler(
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger,
    ActiveScraperJournalsCache cache
) : ICommandHandler<CompleteScraperJournalCommand>
{
    public async Task Handle(CompleteScraperJournalCommand command, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        Guid identifier = await cache.GetIdentifier(command.ParserName, command.ParserType);
        IScraperJournals journals = new NpgSqlScraperJournals(connection);
        ScraperJournal journal = await journals.ById(identifier, ct);
        DateTime completionDate = DateTime.UtcNow;
        journal = journal.Complete(completionDate);
        await using NpgsqlCommand sqlCommad = connection.CreateCommand();
        NpgSqlSavingScraperJournalOutputSource sqlOutput = new(sqlCommad);
        sqlOutput = journal.PrintTo(sqlOutput);
        await sqlOutput.BehaveAsync();
        await sqlCommad.ExecuteNonQueryAsync(ct);
        LoggingScraperJournalOutputSource log = new(logger);
        log = journal.PrintTo(log);
        await log.BehaveAsync();
        logger.Information(
            "Journal {id} completed {Date}.",
            identifier,
            completionDate.ToString(CultureInfo.InvariantCulture)
        );
    }
}
