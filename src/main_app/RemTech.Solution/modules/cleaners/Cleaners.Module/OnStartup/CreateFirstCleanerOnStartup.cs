using System.Data.Common;
using Cleaners.Module.Services.Features.Common;
using Cleaners.Module.Services.Features.CreateNew;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace Cleaners.Module.OnStartup;

internal sealed class CreateFirstCleanerOnStartup(
    Serilog.ILogger logger,
    NpgsqlDataSource dataSource
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            stoppingToken
        );
        if (!await ShouldCreateCleaner(connection, stoppingToken))
        {
            logger.Information(
                "{Entrance}. No need to create cleaner.",
                nameof(CreateFirstCleanerOnStartup)
            );
            return;
        }

        CreateNewCleaner create = new CreateNewCleaner();
        CreateNewCleanerHandler handler = new CreateNewCleanerHandler(logger);
        SavingCleanerHandler<CreateNewCleaner> saving = new SavingCleanerHandler<CreateNewCleaner>(
            connection,
            handler
        );
        LoggingCleanerHandler<CreateNewCleaner> logging =
            new LoggingCleanerHandler<CreateNewCleaner>(logger, saving);
        await logging.Handle(create, stoppingToken);
    }

    private const string CountSql = "SELECT COUNT(*) as amount FROM cleaners_module.cleaners;";

    private async Task<bool> ShouldCreateCleaner(NpgsqlConnection connection, CancellationToken ct)
    {
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = CountSql;
        await using DbDataReader reader = await command.ExecuteReaderAsync(ct);
        await reader.ReadAsync(ct);
        long amount = reader.GetInt64(0);
        return amount == 0;
    }
}
