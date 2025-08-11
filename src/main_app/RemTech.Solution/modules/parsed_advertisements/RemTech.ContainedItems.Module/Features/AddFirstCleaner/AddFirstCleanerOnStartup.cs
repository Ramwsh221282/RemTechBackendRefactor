using Microsoft.Extensions.Hosting;
using Npgsql;
using RemTech.ContainedItems.Module.Types;

namespace RemTech.ContainedItems.Module.Features.AddFirstCleaner;

internal sealed class AddFirstCleanerOnStartup(NpgsqlDataSource dataSource, Serilog.ILogger logger)
    : BackgroundService
{
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
        string sql = string.Intern("SELECT COUNT(id) FROM contained_items.cleaners");
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        object? count = await command.ExecuteScalarAsync(stoppingToken);
        long number = (long)count!;
        return number == 0;
    }

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
        string sql = string.Intern(
            """
            INSERT INTO contained_items.cleaners
            (id, cleaned_amount, last_run, next_run, wait_days, state)
            VALUES
            (@id, @cleaned_amount, @last_run, @next_run, @wait_days, @state)                
            """
        );
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.Add(new NpgsqlParameter<Guid>("@id", cleaner.Id));
        command.Parameters.Add(new NpgsqlParameter<long>("@cleaned_amount", cleaner.CleanedAmount));
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@last_run", cleaner.LastRun));
        command.Parameters.Add(new NpgsqlParameter<DateTime>("@next_run", cleaner.NextRun));
        command.Parameters.Add(new NpgsqlParameter<int>("@wait_days", cleaner.WaitDays));
        command.Parameters.Add(new NpgsqlParameter<string>("@state", cleaner.State));
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
