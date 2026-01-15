using Dapper;
using Microsoft.Extensions.Hosting;
using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.Infrastructure.Common.BackgroundServices;

public sealed class AccountsModuleOutboxCleaner(
    Serilog.ILogger logger,
    NpgSqlConnectionFactory connectionFactory
) : BackgroundService
{
    private Serilog.ILogger Logger { get; } = logger.ForContext<AccountsModuleOutboxCleaner>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Execute(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task Execute(CancellationToken ct = default)
    {
        await using NpgSqlSession session = new(connectionFactory);
        NpgSqlTransactionSource source = new(session);
        await using ITransactionScope transaction = await source.BeginTransaction(ct);
        try
        {
            int removed = await RemoveSentMessages(session, ct);
            Result commit = await transaction.Commit(ct);

            if (commit.IsFailure)
                Logger.Fatal(commit.Error, "Error committing transaction.");
            else
                Logger.Information("Removed {Count} sent outbox messages.", removed);
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Error at cleaning up identity outbox table.");
        }
    }

    private static async Task<int> RemoveSentMessages(
        NpgSqlSession session,
        CancellationToken ct = default
    )
    {
        const string sql = """
            DELETE FROM identity_module.outbox
            WHERE sent is not NULL
            """;

        CommandDefinition command = new(
            sql,
            cancellationToken: ct,
            transaction: session.Transaction
        );

        NpgsqlConnection connection = await session.GetConnection(ct: ct);
        return await connection.ExecuteAsync(command);
    }
}
