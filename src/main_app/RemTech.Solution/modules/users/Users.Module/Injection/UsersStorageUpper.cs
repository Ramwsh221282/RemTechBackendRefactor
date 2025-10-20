using DbUp;
using DbUp.Engine;
using Microsoft.Extensions.Options;
using RemTech.Shared.Configuration.Options;
using Shared.Infrastructure.Module.Postgres;
using Users.Module.Features.CreatingNewAccount;

namespace Users.Module.Injection;

public sealed class UsersStorageUpper : IStorageUpper
{
    private readonly IOptions<DatabaseOptions> _options;

    public UsersStorageUpper(IOptions<DatabaseOptions> options) => _options = options;

    public Task UpStorage()
    {
        string connectionString = _options.Value.ToConnectionString();

        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(JwtUserResult).Assembly)
            .LogToConsole()
            .Build();

        DatabaseUpgradeResult result = upgrader.PerformUpgrade();

        return !result.Successful
            ? throw new ApplicationException("Failed to create users module database.")
            : Task.CompletedTask;
    }
}
