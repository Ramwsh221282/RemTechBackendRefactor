using System.Reflection;
using DbUp;
using DbUp.Engine;
using Npgsql;

namespace Shared.Infrastructure.Module.Postgres.DataAccessConfiguration;

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
        await using NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(
            _configuration.ConnectionString
        ).Build();
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync();
        string joinedTableNames = string.Join(", ", tables);
        await using NpgsqlCommand command = connection.CreateCommand();
        command.CommandText = $"DROP TABLE IF EXISTS {joinedTableNames}, schemaversions CASCADE";
        await command.ExecuteNonQueryAsync();
    }
}
