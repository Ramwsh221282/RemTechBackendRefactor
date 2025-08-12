using Microsoft.Extensions.Hosting;
using Npgsql;
using RemTech.ContainedItems.Module.Types;

namespace RemTech.ContainedItems.Module.Features.AddFirstCleaner;

internal sealed class AddFirstCleanerOnStartup(NpgsqlDataSource dataSource, Serilog.ILogger logger)
    : BackgroundService
{
    private const string CountQuery = "SELECT COUNT(id) FROM contained_items.cleaners";

    private const string InsertQuery = """
        INSERT INTO contained_items.cleaners
        (id, cleaned_amount, last_run, next_run, wait_days, state)
        VALUES
        (@id, @cleaned_amount, @last_run, @next_run, @wait_days, @state)                
        """;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            stoppingToken
        );
        if (await ShouldCreateFirstCleaner(connection, stoppingToken))
            await CreateFirstCleaner(connection, stoppingToken);
    }

    private async Task<bool> ShouldCreateFirstCleaner(
        NpgsqlConnection connection,
        CancellationToken stoppingToken
    )
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = CountQuery;
        object? count = await command.ExecuteScalarAsync(stoppingToken);
        long number = (long)count!;
        return number == 0;
    }

    private const string IdParam = "@id";
    private const string CleanedAmountParam = "@cleaned_amount";
    private const string LastRunParam = "@last_run";
    private const string NextRunParam = "@next_run";
    private const string WaitDaysParam = "@wait_days";
    private const string StateParam = "@state";

    private async Task CreateFirstCleaner(
        NpgsqlConnection connection,
        CancellationToken stoppingToken
    )
    {
        IContainedItemsCleaner cleaner = new ContainedItemsClear(
            Guid.NewGuid(),
            0,
            DateTime.UtcNow,
            DateTime.UtcNow,
            1,
            "Отключен"
        );
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = InsertQuery;
        command.Parameters.Add(new NpgsqlParameter<Guid>(IdParam, cleaner.Id));
        command.Parameters.Add(
            new NpgsqlParameter<long>(CleanedAmountParam, cleaner.CleanedAmount)
        );
        command.Parameters.Add(new NpgsqlParameter<DateTime>(LastRunParam, cleaner.LastRun));
        command.Parameters.Add(new NpgsqlParameter<DateTime>(NextRunParam, cleaner.NextRun));
        command.Parameters.Add(new NpgsqlParameter<int>(WaitDaysParam, cleaner.WaitDays));
        command.Parameters.Add(new NpgsqlParameter<string>(StateParam, cleaner.State));
        int affected = await command.ExecuteNonQueryAsync(stoppingToken);
        if (affected == 0)
        {
            logger.Fatal(
                "{Entrance} unable to create first cleaner.",
                nameof(AddFirstCleanerOnStartup)
            );
            throw new ApplicationException("Unable to create first cleaner.");
        }
        logger.Information(
            "{Entrance} first cleaner has been added.",
            nameof(AddFirstCleanerOnStartup)
        );
    }
}
