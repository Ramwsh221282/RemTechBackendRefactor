using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace Shared.Infrastructure.Module.Postgres;

public static class DbUpDatabaseUp
{
    public static void UpDatabase(string connectionString, Assembly from)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(from)
            .LogToConsole()
            .Build();

        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException($"Failed to create {from.GetName().Name} database.");
    }
}
