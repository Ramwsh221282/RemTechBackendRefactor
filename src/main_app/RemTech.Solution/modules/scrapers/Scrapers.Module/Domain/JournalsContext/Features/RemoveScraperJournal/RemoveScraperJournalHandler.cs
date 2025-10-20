using System.Data.Common;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Scrapers.Module.Domain.JournalsContext.Exceptions;
using Scrapers.Module.Domain.JournalsContext.Logging;
using Scrapers.Module.Domain.JournalsContext.Persistance;

namespace Scrapers.Module.Domain.JournalsContext.Features.RemoveScraperJournal;

internal sealed class RemoveScraperJournalHandler(
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger
) : ICommandHandler<RemoveScraperJournalCommand, Guid>
{
    public async Task<Guid> Handle(
        RemoveScraperJournalCommand command,
        CancellationToken ct = default
    )
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(ct);
        if (await IsScraperWorking(command.ScraperName, command.ScraperType, connection, ct))
            throw new CannotRemoveNotCompletedScraperJournalException();
        IScraperJournals journals = new NpgSqlScraperJournals(connection);
        ScraperJournal journal = await journals.ById(command.Id, ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        NpgSqlRemovingScraperJournalOutput sqlOutput = new(sqlCommand);
        sqlOutput = journal.PrintTo(sqlOutput);
        await sqlOutput.BehaveAsync();
        int affected = await sqlCommand.ExecuteNonQueryAsync(ct);
        if (affected == 0)
            throw new ScraperJournalByIdNotFoundException();
        LoggingScraperJournalOutputSource log = new(logger);
        log = journal.PrintTo(log);
        await log.BehaveAsync();
        logger.Information("Удален");
        return command.Id;
    }

    private const string GetScraperStateSql =
        "SELECT state FROM scrapers_module.scrapers WHERE name = @name AND type = @type;";

    private static async Task<bool> IsScraperWorking(
        string scraperName,
        string scraperType,
        NpgsqlConnection connection,
        CancellationToken ct
    )
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = GetScraperStateSql;
        command.Parameters.Add(new NpgsqlParameter<string>("@name", scraperName));
        command.Parameters.Add(new NpgsqlParameter<string>("@type", scraperType));
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            throw new ScraperJournalByIdNotFoundException();
        string state = reader.GetString(0);
        return state == "Работает";
    }
}
