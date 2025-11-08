using System.Reflection;
using DbUp.Engine;
using Microsoft.Extensions.Options;

namespace RemTech.NpgSql.Abstractions;

public abstract class AbstractDatabaseUpgrader(IOptions<NpgSqlOptions> options) : IDbUpgrader
{
    private readonly string _connectionString = options.Value.ToConnectionString();
    private Assembly? _assembly;

    protected void OfAssembly(Assembly assembly) => _assembly = assembly;

    public void ApplyMigrations()
    {
        UpgradeEngine upgrader = DbUp.DeployChanges.To.PostgresqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(_assembly)
            .LogToConsole()
            .Build();

        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful) throw new ApplicationException("Cannot apply migrations for database.");
        Console.WriteLine($"Successfully applied migrations for database.");
    }
}