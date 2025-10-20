using DbUp;
using DbUp.Engine;
using Mailing.Module.Sources.NpgSql;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Module.Injection;

public sealed class MailingModuleStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public MailingModuleStorageUpper(IOptions<DatabaseOptions> options) => _options = options;

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();

        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(NpgSqlEmailSendersSource).Assembly)
            .LogToConsole()
            .Build();

        DatabaseUpgradeResult result = upgrader.PerformUpgrade();

        return !result.Successful
            ? throw new ApplicationException("Failed to create parsers management database.")
            : Task.CompletedTask;
    }
}