using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Scrapers.Module.Domain.JournalsContext.Cache;
using Scrapers.Module.Domain.JournalsContext.Persistance;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Scrapers.Module.Domain.JournalsContext.Features.AddJournalRecord;

internal sealed class AddJournalRecordHandler(
    NpgsqlDataSource dataSource,
    IEmbeddingGenerator generator,
    ActiveScraperJournalsCache cache
) : ICommandHandler<AddJournalRecordCommand>
{
    public async Task Handle(AddJournalRecordCommand command, CancellationToken ct = default)
    {
        Guid identifier = await cache.GetIdentifier(command.ParserName, command.ParserType);
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        IScraperJournals journals = new NpgSqlScraperJournals(connection);
        ScraperJournal journal = await journals.ById(identifier, ct);
        ScraperJournalRecord record = journal.WithRecord(
            Guid.NewGuid(),
            command.Action,
            command.Text,
            DateTime.UtcNow
        );
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        NpgSqlSavingScraperJournalRecordOutputSource sqlOutput = new(sqlCommand, generator);
        sqlOutput = record.PrintTo(sqlOutput);
        await sqlOutput.BehaveAsync();
        await sqlCommand.ExecuteNonQueryAsync(ct);
    }
}
