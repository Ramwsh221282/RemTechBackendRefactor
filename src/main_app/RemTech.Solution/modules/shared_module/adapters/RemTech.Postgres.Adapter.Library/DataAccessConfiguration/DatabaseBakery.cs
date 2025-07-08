using System.Reflection;
using Dapper;
using DbUp;
using DbUp.Engine;
using Npgsql;

namespace RemTech.Postgres.Adapter.Library.DataAccessConfiguration;

public sealed class DatabaseBakery
{
    private readonly DatabaseConfiguration _configuration;

    public DatabaseBakery(DatabaseConfiguration configuration) => _configuration = configuration;

    public void Up(Assembly assembly)
    {
        string connectionString = _configuration.ConnectionString;
        EnsureDatabase.For.PostgresqlDatabase(connectionString);
        UpgradeEngine upgrader = DeployChanges
            .To.PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(assembly)
            .LogToConsole()
            .Build();
        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
            throw new ApplicationException("Failed to create parsers management database.");
    }

    public async Task Down(params string[] tables)
    {
        if (tables.Length == 0)
            throw new ApplicationException("Tables cannot be empty.");
        await using PostgreSqlEngine engine = new(_configuration);
        NpgsqlConnection connection = await engine.Connect();
        string joinedTableNames = string.Join(", ", tables);
        await connection.ExecuteAsync($"DROP TABLE IF EXISTS {joinedTableNames}, schemaversions");
    }
}
